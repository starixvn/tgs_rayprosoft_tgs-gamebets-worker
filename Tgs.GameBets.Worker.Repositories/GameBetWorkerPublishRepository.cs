using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Client;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Repositories;

internal class GameBetWorkerPublishRepository(
	ILogger<GameBetWorkerPublishRepository> logger,
	IMessageSender messageSender) : IGameBetWorkerPublishRepository
{
	public async ValueTask PublishAsync(GameBetWorkerPublish source, CancellationToken cancellationToken = default)
	{
		if (source.Result == GameBets.Common.Enums.GameBetCommandResult.Running)
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						source.GameProviderId,
						source.GameId,
						source.PlayerCode,
						source.WalletId,
						source.ExternalBetId,
						ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
						source.Result
					},
					Message = "The bet status of this game is not settled."
				}));

			return;
		}

		var publish = new Common.Protos.GameBetsWorkerPublish
		{
			GameBetId = source.GameBetId,
			GameProviderId = source.GameProviderId,
			GameProviderConfigurationId = source.GameProviderConfigurationId,
			GameId = source.GameId,
			ResellerId = source.ResellerId,
			LicenseeId = source.LicenseeId,
			BrandId = source.BrandId,
			PlayerCode = source.PlayerCode,
			CurrencyCode = source.CurrencyCode,
			WalletId = source.WalletId,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
			ExternalClosedOn = source.ExternalClosedOn.ToTimestamp(),
			ExternalPayoffOn = source.ExternalPayoffOn.ToTimestamp(),
			ExternalResettleOn = source.ExternalResettleOn.ToTimestamp(),
			BetState = (Common.Protos.BetState)source.BetState,
			BetType = (Common.Protos.BetType)source.BetType,
			FreeGameState = (Common.Protos.FreeGameState)source.FreeGameState,
			GambleState = (Common.Protos.GambleState)source.GambleState,
			CashoutState = (Common.Protos.CashoutState)source.CashoutState,
			ResettleState = (Common.Protos.ResettleState)source.ResettleState,
			JackpotState = (Common.Protos.JackpotState)source.JackpotState,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			Jackpot = source.Jackpot,
			CreatedOn = source.CreatedOn.ToTimestamp(),
			ClosedOn = source.ClosedOn.ToTimestamp(),
			UpdatedOn = source.ClosedOn.ToTimestamp()
		};

		await messageSender.PublishAsync(
			subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish,
			data: publish,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish,
				Data = new
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
					ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This game bet is published."
			}));

		if (source.Result == GameBets.Common.Enums.GameBetCommandResult.Add)
		{
			await messageSender.PublishAsync(
				subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish,
				data: publish,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("{logInformation}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Subject = Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish,
					Data = new
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
						ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset().ToString("u")
					},
					Message = "This game bet is added and published."
				}));
		}

		if (source.Result == GameBets.Common.Enums.GameBetCommandResult.Update)
		{
			await messageSender.PublishAsync(
				subject: Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish,
				data: publish,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("{logInformation}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Subject = Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish,
					Data = new
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
						ExternalCreatedOn = publish.ExternalCreatedOn.ToDateTimeOffset().ToString("u")
					},
					Message = "This game bet is updateed and published."
				}));
		}
	}
}
