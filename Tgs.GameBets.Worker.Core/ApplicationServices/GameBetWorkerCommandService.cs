using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerCommandService(
	ICommandService<GameBetCommand> gameBetService,
	ICommandService<GameBetFirstCommand> gameBetFirstService) : ICommandService<GameBetWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		if (command.BetState is Common.Protos.BetState.Waiting or Common.Protos.BetState.Running)
		{
			return;
		}

		await gameBetService.ExecuteAsync(
		command: new GameBetCommand(
			GameBetId: command.GameBetId,
			GameProviderId: command.GameProviderId,
			GameProviderConfigurationId: command.GameProviderConfigurationId,
			GameId: command.GameId,
			ResellerId: command.ResellerId,
			LicenseeId: command.LicenseeId,
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			CurrencyCode: command.CurrencyCode,
			WalletId: command.WalletId,
			ExternalRoundId: command.ExternalRoundId,
			ExternalBetId: command.ExternalBetId,
			ExternalCreatedOn: command.ExternalCreatedOn,
			ExternalClosedOn: command.ExternalClosedOn,
			ExternalPayoffOn: command.ExternalPayoffOn,
			ExternalResettleOn: command.ExternalResettleOn,
			BetState: command.BetState,
			BetType: command.BetType,
			FreeGameState: command.FreeGameState,
			GambleState: command.GambleState,
			CashoutState: command.CashoutState,
			ResettleState: command.ResettleState,
			JackpotState: command.JackpotState,
			CurrencyRatio: command.CurrencyRatio,
			Bet: command.Bet,
			ValidBet: command.ValidBet,
			Settle: command.Settle,
			Commission: command.Commission,
			Turnover: command.Turnover,
			WinLoss: command.WinLoss,
			Jackpot: command.Jackpot,
			AdditionalData: command.AdditionalData),
		cancellationToken).ConfigureAwait(false);

		await gameBetFirstService.ExecuteAsync(
		command: new GameBetFirstCommand(
			GameProviderId: command.GameProviderId,
			GameId: command.GameId,
			PlayerCode: command.PlayerCode,
			WalletId: command.WalletId,
			ExternalBetId: command.ExternalBetId,
			ExternalCreatedOn: command.ExternalCreatedOn),
		cancellationToken).ConfigureAwait(false);
	}
}
