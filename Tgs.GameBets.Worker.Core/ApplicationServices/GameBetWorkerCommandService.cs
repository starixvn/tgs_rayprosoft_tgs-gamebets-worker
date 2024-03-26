using Tgs.GameBets.Queue.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerCommandService(
	IGameBetQueueClient client,
	ICommandService<GameBetHandlerCommand> gameBetHandlerService) : ICommandService<GameBetWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		var pageSize = 100;
		IEnumerable<Queue.Common.ViewModels.GameBetQueueInfoViewModel> gameBetQueueInfos;

		gameBetQueueInfos = await client.QueryAsync(
			gameProviderCode: command.GameProviderCode,
			pageIndex: command.PageIndex,
			pageSize: pageSize,
			cancellationToken: cancellationToken)
			.ToArrayAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<Queue.Common.ViewModels.GameBetQueueInfoViewModel>();

		foreach (var gameBetQueueInfo in gameBetQueueInfos)
		{
			await gameBetHandlerService.ExecuteAsync(
			command: new GameBetHandlerCommand(
				GameProviderId: gameBetQueueInfo.GameProviderId,
				GameProviderConfigurationId: gameBetQueueInfo.GameProviderConfigurationId,
				GameId: gameBetQueueInfo.GameId,
				ResellerId: gameBetQueueInfo.ResellerId,
				LicenseeId: gameBetQueueInfo.LicenseeId,
				BrandId: gameBetQueueInfo.BrandId,
				PlayerCode: gameBetQueueInfo.PlayerCode,
				CurrencyCode: gameBetQueueInfo.CurrencyCode,
				WalletId: gameBetQueueInfo.WalletId,
				ExternalRoundId: gameBetQueueInfo.ExternalRoundId,
				ExternalBetId: gameBetQueueInfo.ExternalBetId,
				ExternalCreatedOn: gameBetQueueInfo.ExternalCreatedOn,
				ExternalClosedOn: gameBetQueueInfo.ExternalClosedOn,
				ExternalPayoffOn: gameBetQueueInfo.ExternalPayoffOn,
				ExternalResettleOn: gameBetQueueInfo.ExternalResettleOn,
				BetState: gameBetQueueInfo.BetState,
				BetType: gameBetQueueInfo.BetType,
				FreeGameState: gameBetQueueInfo.FreeGameState,
				GambleState: gameBetQueueInfo.GambleState,
				CashoutState: gameBetQueueInfo.CashoutState,
				ResettleState: gameBetQueueInfo.ResettleState,
				JackpotState: gameBetQueueInfo.JackpotState,
				CurrencyRatio: gameBetQueueInfo.CurrencyRatio,
				Bet: gameBetQueueInfo.Bet,
				ValidBet: gameBetQueueInfo.ValidBet,
				Settle: gameBetQueueInfo.Settle,
				Commission: gameBetQueueInfo.Commission,
				Turnover: gameBetQueueInfo.Turnover,
				WinLoss: gameBetQueueInfo.WinLoss,
				Jackpot: gameBetQueueInfo.Jackpot,
				AdditionalData: gameBetQueueInfo.AdditionalData),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

			await client.DeleteAsync(
				gameProviderCode: command.GameProviderCode,
				id: gameBetQueueInfo.Id,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
