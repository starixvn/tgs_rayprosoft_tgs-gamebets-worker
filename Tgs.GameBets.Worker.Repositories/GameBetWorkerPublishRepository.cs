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
	IGameBetsClient client,
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

		var model = await client.GameBetClosesGetAsync(
			externalCreatedOn: source.ExternalCreatedOn,
			gameProviderId: source.GameProviderId,
			gameId: source.GameId,
			walletId: source.WalletId,
			externalBetId: source.ExternalBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? await client.GameBetMajorGetAsync(
				externalCreatedOn: source.ExternalCreatedOn,
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				new
				{
					source.GameProviderId,
					source.GameId,
					source.PlayerCode,
					source.WalletId,
					source.ExternalBetId,
					source.ExternalCreatedOn
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
