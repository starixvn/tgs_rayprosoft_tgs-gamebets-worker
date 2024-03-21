using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
internal class GameBetHandlerCommandServic(
	ICommandService<GameBetCommand> gameBetService,
	ICommandService<GameBetFirstCommand> gameBetFirstService) : ICommandService<GameBetHandlerCommand>
{
	public async ValueTask ExecuteAsync(GameBetHandlerCommand command, CancellationToken cancellationToken = default)
	{
		await gameBetService.ExecuteAsync(
			command: new GameBetCommand(
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
				ExternalWagerId: command.ExternalWagerId,
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
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameBetFirstService.ExecuteAsync(
			command: new GameBetFirstCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalWagerId: command.ExternalWagerId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				BetState: command.BetState),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
