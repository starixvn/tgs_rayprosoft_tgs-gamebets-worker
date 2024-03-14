using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
internal class GameWagerSettlementWorkerCommandService(
	IGameProviderRepository gameProviderRepository,
	IGameWagerMarkerRepository gameWagerMarkerRepository,
	IGameWagersClient client,
	ICommandService<GameWagerSettlementCommand> settlementService,
	ICommandService<GameWagerSettlementFirstCommand> settlementFirstService) : ICommandService<GameWagerSettlementWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameWagerSettlementWorkerCommand command, CancellationToken cancellationToken = default)
	{
		var markerCode = string.Concat(command.GameProviderCode, "-", "settlement").ToUpper();
		var gameProviderId = await gameProviderRepository.GetAsync(
			code: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameWagerMarkerInfo = await gameWagerMarkerRepository.GetAsync(
			code: markerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameWagerMarkerInfo is null)
		{
			await gameWagerMarkerRepository.AddAsync(
				code: markerCode,
				directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			gameWagerMarkerInfo = await gameWagerMarkerRepository.GetAsync(
				code: markerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(markerCode);
		}

		Guid marker = gameWagerMarkerInfo.Marker;
		IEnumerable<GameWagersEventSourceInfo>? gameWagersEventSourceInfos;
		do
		{
			gameWagersEventSourceInfos = await client.GameWagersEventSourceQuerySettlementsAsync(
				gameProviderId: gameProviderId,
				id: marker,
				limit: 100,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagersEventSourceInfo>();

			foreach (var gameWagersEventSourceInfo in gameWagersEventSourceInfos)
			{
				await settlementService.ExecuteAsync(
					command: new GameWagerSettlementCommand(
						GameWagerPrimaryId: gameWagersEventSourceInfo.GameWagerPrimaryId,
						WagerState: gameWagersEventSourceInfo.WagerState,
						WagerType: gameWagersEventSourceInfo.WagerType,
						FreeGameState: gameWagersEventSourceInfo.FreeGameState,
						GambleState: gameWagersEventSourceInfo.GambleState,
						CashoutState: gameWagersEventSourceInfo.CashoutState,
						ResettleState: gameWagersEventSourceInfo.ResettleState,
						CurrencyRatio: gameWagersEventSourceInfo.CurrencyRatio,
						Bet: gameWagersEventSourceInfo.Bet,
						ValidBet: gameWagersEventSourceInfo.ValidBet,
						Settle: gameWagersEventSourceInfo.Settle,
						Commission: gameWagersEventSourceInfo.Commission,
						Turnover: gameWagersEventSourceInfo.Turnover,
						WinLoss: gameWagersEventSourceInfo.WinLoss,
						ExternalCreatedOn: gameWagersEventSourceInfo.ExternalCreatedOn,
						ExternalClosedOn: gameWagersEventSourceInfo.ExternalClosedOn,
						ExternalPayoffOn: gameWagersEventSourceInfo.ExternalPayoffOn,
						ExternalResettleOn: gameWagersEventSourceInfo.ExternalResettleOn),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await settlementFirstService.ExecuteAsync(
					command: new GameWagerSettlementFirstCommand(
						GameWagerPrimaryId: gameWagersEventSourceInfo.GameWagerPrimaryId,
						ExternalCreatedOn: gameWagersEventSourceInfo.ExternalCreatedOn),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				marker = gameWagersEventSourceInfo.Id;
			}

			await gameWagerMarkerRepository.UpdateAsync(
				code: markerCode,
				marker: marker,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		} while (gameWagersEventSourceInfos.Any());

	}
}
