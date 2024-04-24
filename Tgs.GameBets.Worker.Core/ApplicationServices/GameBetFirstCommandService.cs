using Google.Protobuf.WellKnownTypes;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetFirstCommandService(
	IGameBetRepository gameBetRepository,
	IMessageSender messageSender) : ICommandService<GameBetFirstCommand>
{
	public async ValueTask ExecuteAsync(GameBetFirstCommand command, CancellationToken cancellationToken = default)
	{
		var model = await gameBetRepository.GetAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(command.ToString());

		await messageSender.PublishAsync(
			subject: First.Worker.Common.GameBetsFirstWorkerNatsConst.GameBetsFirstWorkerRequest,
			data: new First.Worker.Common.Protos.GameBetsFirstWorkerRequest
			{
				GameBetId = model.Id,
				GameProviderId = model.GameProviderId,
				GameProviderConfigurationId = model.GameProviderConfigurationId,
				GameId = model.GameId,
				ResellerId = model.ResellerId,
				LicenseeId = model.LicenseeId,
				BrandId = model.BrandId,
				PlayerCode = model.PlayerCode,
				CurrencyCode = model.CurrencyCode,
				WalletId = model.WalletId,
				ExternalRoundId = model.ExternalRoundId,
				ExternalBetId = model.ExternalBetId,
				ExternalCreatedOn = model.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = model.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = model.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = model.ExternalResettleOn.ToTimestamp(),
				BetState = (First.Worker.Common.Protos.BetState)model.BetState,
				BetType = (First.Worker.Common.Protos.BetType)model.BetType,
				FreeGameState = (First.Worker.Common.Protos.FreeGameState)model.FreeGameState,
				GambleState = (First.Worker.Common.Protos.GambleState)model.GambleState,
				CashoutState = (First.Worker.Common.Protos.CashoutState)model.CashoutState,
				ResettleState = (First.Worker.Common.Protos.ResettleState)model.ResettleState,
				JackpotState = (First.Worker.Common.Protos.JackpotState)model.JackpotState,
				CurrencyRatio = model.CurrencyRatio,
				Bet = model.Bet,
				ValidBet = model.ValidBet,
				Settle = model.Settle,
				Commission = model.Commission,
				Turnover = model.Turnover,
				WinLoss = model.WinLoss,
				Jackpot = model.Jackpot,
				CreatedOn = model.CreatedOn.ToTimestamp(),
				ClosedOn = model.ClosedOn.ToTimestamp(),
				UpdatedOn = model.UpdatedOn.ToTimestamp(),
				AdditionalData = model.AdditionalData.ToJsonString()
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
