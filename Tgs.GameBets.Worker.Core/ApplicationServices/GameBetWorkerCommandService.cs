using Tgs.GameBets.Queue.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerCommandService(
	IGameBetQueueClient client,
	ICommandService<GameBetHandlerCommand> gameBetHandlerService) : ICommandService<GameBetWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		GameBets.Queue.Common.ViewModels.GameBetQueueInfoViewModel? gameBetQueueInfo;

		do
		{
			gameBetQueueInfo = await client.PopAsync(
				gpCode: command.GameProviderCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameBetQueueInfo is null)
			{
				break;
			}

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

			await client.RemoveAsync(
				gpCode: command.GameProviderCode,
				source: new Queue.Common.ViewModels.GameBetQueueRemoveViewModel
				{
					GameProviderId = gameBetQueueInfo.GameProviderId,
					GameProviderConfigurationId = gameBetQueueInfo.GameProviderConfigurationId,
					GameId = gameBetQueueInfo.GameId,
					ResellerId = gameBetQueueInfo.ResellerId,
					LicenseeId = gameBetQueueInfo.LicenseeId,
					BrandId = gameBetQueueInfo.BrandId,
					PlayerCode = gameBetQueueInfo.PlayerCode,
					CurrencyCode = gameBetQueueInfo.CurrencyCode,
					WalletId = gameBetQueueInfo.WalletId,
					ExternalRoundId = gameBetQueueInfo.ExternalRoundId,
					ExternalBetId = gameBetQueueInfo.ExternalBetId,
					ExternalCreatedOn = gameBetQueueInfo.ExternalCreatedOn,
					ExternalClosedOn = gameBetQueueInfo.ExternalClosedOn,
					ExternalPayoffOn = gameBetQueueInfo.ExternalPayoffOn,
					ExternalResettleOn = gameBetQueueInfo.ExternalResettleOn,
					BetState = gameBetQueueInfo.BetState,
					BetType = gameBetQueueInfo.BetType,
					FreeGameState = gameBetQueueInfo.FreeGameState,
					GambleState = gameBetQueueInfo.GambleState,
					CashoutState = gameBetQueueInfo.CashoutState,
					ResettleState = gameBetQueueInfo.ResettleState,
					JackpotState = gameBetQueueInfo.JackpotState,
					CurrencyRatio = gameBetQueueInfo.CurrencyRatio,
					Bet = gameBetQueueInfo.Bet,
					ValidBet = gameBetQueueInfo.ValidBet,
					Settle = gameBetQueueInfo.Settle,
					Commission = gameBetQueueInfo.Commission,
					Turnover = gameBetQueueInfo.Turnover,
					WinLoss = gameBetQueueInfo.WinLoss,
					Jackpot = gameBetQueueInfo.Jackpot,
					AdditionalData = gameBetQueueInfo.AdditionalData
				},
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		} while (gameBetQueueInfo is not null);
	}
}
