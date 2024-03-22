using Tgs.GameWagers.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
internal class GameBetWorkerCommandService(
	IGameProviderRepository gameProviderRepository,
	IGameBetMarkerRepository gameBetMarkerRepository,
	IGameWagersClient client,
	ICommandService<GameBetHandlerCommand> gameBetHandlerService) : ICommandService<GameBetWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetAsync(
			code: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameBetMarkerInfo = await gameBetMarkerRepository.GetAsync(
			code: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameBetMarkerInfo is null)
		{
			await gameBetMarkerRepository.AddAsync(
				code: command.GameProviderCode,
				directions: string.Concat(command.GameProviderCode, " Game Bet Worker"),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			gameBetMarkerInfo = await gameBetMarkerRepository.GetAsync(
				code: command.GameProviderCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(command.GameProviderCode);
		}

		Guid marker = gameBetMarkerInfo.Marker;
		IEnumerable<GameWagers.Common.GameBetsEventSourceInfo>? gameBetsEventSourceInfos;
		do
		{
			gameBetsEventSourceInfos = await client.GameBetsEventSourceQueryAllAsync(
				gameProviderId: gameProviderId,
				id: marker,
				limit: 100,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagers.Common.GameBetsEventSourceInfo>();

			foreach (var gameBetsEventSourceInfo in gameBetsEventSourceInfos)
			{
				await gameBetHandlerService.ExecuteAsync(
					command: new GameBetHandlerCommand(
						GameProviderId: gameBetsEventSourceInfo.GameProviderId,
						GameProviderConfigurationId: gameBetsEventSourceInfo.GameProviderConfigurationId,
						GameId: gameBetsEventSourceInfo.GameId,
						ResellerId: gameBetsEventSourceInfo.ResellerId,
						LicenseeId: gameBetsEventSourceInfo.LicenseeId,
						BrandId: gameBetsEventSourceInfo.BrandId,
						PlayerCode: gameBetsEventSourceInfo.PlayerCode,
						CurrencyCode: gameBetsEventSourceInfo.CurrencyCode,
						WalletId: gameBetsEventSourceInfo.WalletId,
						ExternalRoundId: gameBetsEventSourceInfo.ExternalRoundId,
						ExternalBetId: gameBetsEventSourceInfo.ExternalBetId,
						ExternalCreatedOn: gameBetsEventSourceInfo.ExternalCreatedOn,
						ExternalClosedOn: gameBetsEventSourceInfo.ExternalClosedOn,
						ExternalPayoffOn: gameBetsEventSourceInfo.ExternalPayoffOn,
						ExternalResettleOn: gameBetsEventSourceInfo.ExternalResettleOn,
						BetState: gameBetsEventSourceInfo.BetState,
						BetType: gameBetsEventSourceInfo.BetType,
						FreeGameState: gameBetsEventSourceInfo.FreeGameState,
						GambleState: gameBetsEventSourceInfo.GambleState,
						CashoutState: gameBetsEventSourceInfo.CashoutState,
						ResettleState: gameBetsEventSourceInfo.ResettleState,
						JackpotState: gameBetsEventSourceInfo.JackpotState,
						CurrencyRatio: gameBetsEventSourceInfo.CurrencyRatio,
						Bet: gameBetsEventSourceInfo.Bet,
						ValidBet: gameBetsEventSourceInfo.ValidBet,
						Settle: gameBetsEventSourceInfo.Settle,
						Commission: gameBetsEventSourceInfo.Commission,
						Turnover: gameBetsEventSourceInfo.Turnover,
						WinLoss: gameBetsEventSourceInfo.WinLoss,
						Jackpot: gameBetsEventSourceInfo.Jackpot,
						AdditionalData: gameBetsEventSourceInfo.AdditionalData),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				marker = gameBetsEventSourceInfo.Id;
			}

			await gameBetMarkerRepository.UpdateAsync(
				code: command.GameProviderCode,
				marker: marker,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		} while (gameBetsEventSourceInfos.Any());

	}
}
