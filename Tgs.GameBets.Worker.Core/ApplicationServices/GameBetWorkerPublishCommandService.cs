using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerPublishCommandService(
	ILogger<GameBetWorkerPublishCommandService> logger,
	IGameBetRepository gameBetRepository,
	IMessageSender messageSender) : ICommandService<GameBetWorkerPublishCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerPublishCommand command, CancellationToken cancellationToken = default)
	{
		if (command.Result == GameBets.Common.Enums.GameBetCommandResult.Running)
		{
			logger.LogWarning("LogOn:{logOn} Message:The bet status of this game is not settled.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					command.GameProviderId,
					command.GameId,
					command.PlayerCode,
					command.WalletId,
					command.ExternalBetId,
					command.ExternalCreatedOn,
					command.Result
				});
			return;
		}

		var model = await gameBetRepository.GetAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				new
				{
					command.GameProviderId,
					command.GameId,
					command.PlayerCode,
					command.WalletId,
					command.ExternalBetId,
					command.ExternalCreatedOn
				}.ToString(),
				"This game bet does not exist.");

		var publish = new Common.Protos.GameBetsWorkerPublish
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
			BetState = (Common.Protos.BetState)model.BetState,
			BetType = (Common.Protos.BetType)model.BetType,
			FreeGameState = (Common.Protos.FreeGameState)model.FreeGameState,
			GambleState = (Common.Protos.GambleState)model.GambleState,
			CashoutState = (Common.Protos.CashoutState)model.CashoutState,
			ResettleState = (Common.Protos.ResettleState)model.ResettleState,
			JackpotState = (Common.Protos.JackpotState)model.JackpotState,
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
			UpdatedOn = model.UpdatedOn.ToTimestamp()
		};

		await messageSender.PublishAsync(
			subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish,
			data: publish,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This game bet is published.({publish})",
			DateTime.Now.ToString("o"),
			Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish,
			new
			{
				GameBetId = publish.GameBetId.ToGuid(),
				GameProviderId = publish.GameProviderId.ToGuid(),
				publish.GameId,
				ResellerId = publish.ResellerId.ToGuid(),
				LicenseeId = publish.LicenseeId.ToGuid(),
				BrandId = publish.BrandId.ToGuid(),
				publish.PlayerCode,
				publish.CurrencyCode,
				WalletId = publish.WalletId.ToGuid(),
				publish.ExternalRoundId,
				publish.ExternalBetId,
				ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset()
			});

		if (command.Result == GameBets.Common.Enums.GameBetCommandResult.Add)
		{
			await messageSender.PublishAsync(
				subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish,
				data: publish,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This game bet is added and published.({publish})",
				DateTime.Now.ToString("o"),
				Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish,
				new
				{
					GameBetId = publish.GameBetId.ToGuid(),
					GameProviderId = publish.GameProviderId.ToGuid(),
					publish.GameId,
					ResellerId = publish.ResellerId.ToGuid(),
					LicenseeId = publish.LicenseeId.ToGuid(),
					BrandId = publish.BrandId.ToGuid(),
					publish.PlayerCode,
					publish.CurrencyCode,
					WalletId = publish.WalletId.ToGuid(),
					publish.ExternalRoundId,
					publish.ExternalBetId,
					ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset()
				});
		}

		if (command.Result == GameBets.Common.Enums.GameBetCommandResult.Update)
		{
			await messageSender.PublishAsync(
				subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish,
				data: publish,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This game bet is updateed and published.({publish})",
				DateTime.Now.ToString("o"),
				Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish,
				new
				{
					GameBetId = publish.GameBetId.ToGuid(),
					GameProviderId = publish.GameProviderId.ToGuid(),
					publish.GameId,
					ResellerId = publish.ResellerId.ToGuid(),
					LicenseeId = publish.LicenseeId.ToGuid(),
					BrandId = publish.BrandId.ToGuid(),
					publish.PlayerCode,
					publish.CurrencyCode,
					WalletId = publish.WalletId.ToGuid(),
					publish.ExternalRoundId,
					publish.ExternalBetId,
					ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset()
				});
		}
	}
}
