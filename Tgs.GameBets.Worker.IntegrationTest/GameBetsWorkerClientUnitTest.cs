using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.Worker.Client;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.IntegrationTest;

public class GameBetsWorkerClientUnitTest
{
	[Fact]
	public async Task Game_Bet_Handle()
	{
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var createdOn = DateTimeOffset.UtcNow.AddSeconds(-4);
		var closedOn = DateTimeOffset.UtcNow.AddSeconds(-2);

		var source = new Common.ViewModels.GameBetHandleViewModel
		{
			GameBetId = Guid.NewGuid(),
			GameProviderId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			GameId = 1,
			ResellerId = Guid.NewGuid(),
			LicenseeId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			CurrencyCode = "CNY",
			WalletId = Guid.NewGuid(),
			ExternalRoundId = externalRoundId,
			ExternalBetId = externalBetId,
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalPayoffOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = Common.Protos.BetState.Win,
			BetType = Common.Protos.BetType.Normal,
			FreeGameState = Common.Protos.FreeGameState.None,
			GambleState = Common.Protos.GambleState.None,
			CashoutState = Common.Protos.CashoutState.None,
			ResettleState = Common.Protos.ResettleState.None,
			JackpotState = Common.Protos.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = 100.0m,
			ValidBet = 50.0m,
			Settle = 97.50m,
			WinLoss = -2.5m,
			Turnover = 50.0m,
			Commission = 0.1m,
			Contribute = 0.01m,
			Jackpot = 0.0m,
			CreatedOn = createdOn,
			ClosedOn = closedOn,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData = new JsonObject { }
		};

		var fakeMessageSender = Substitute.For<IMessageSender>();
		var web = new GameBetsWorkerApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMessageSender)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameBetsWorkerClient(httpClient);

		await client.HandleAsync(source);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBetsWorkerNatsConst.GameBetCloseWorkerRequest),
			data: Arg.Is<Common.Protos.GameBetCloseWorkerRequest>(compare =>

				compare.GameBetId.ToGuid() == source.GameBetId &&
				compare.GameProviderId.ToGuid() == source.GameProviderId &&
				compare.GameProviderConfigurationId.ToGuid() == source.GameProviderConfigurationId &&
				compare.GameId == source.GameId &&
				compare.ResellerId.ToGuid() == source.ResellerId &&
				compare.LicenseeId.ToGuid() == source.LicenseeId &&
				compare.BrandId.ToGuid() == source.BrandId &&
				compare.PlayerCode == source.PlayerCode &&
				compare.CurrencyCode == source.CurrencyCode &&
				compare.WalletId.ToGuid() == source.WalletId &&
				compare.ExternalRoundId == source.ExternalRoundId &&
				compare.ExternalBetId == source.ExternalBetId &&
				compare.ExternalCreatedOn == source.ExternalCreatedOn.ToTimestamp() &&
				compare.ExternalClosedOn == source.ExternalClosedOn.ToTimestamp() &&
				compare.ExternalPayoffOn == source.ExternalPayoffOn.ToTimestamp() &&
				compare.ExternalResettleOn == source.ExternalResettleOn.ToTimestamp() &&
				compare.BetState == source.BetState &&
				compare.BetType == source.BetType &&
				compare.FreeGameState == source.FreeGameState &&
				compare.GambleState == source.GambleState &&
				compare.CashoutState == source.CashoutState &&
				compare.ResettleState == source.ResettleState &&
				compare.JackpotState == source.JackpotState &&
				compare.CurrencyRatio == source.CurrencyRatio &&
				compare.Bet.ToDecimal() == source.Bet &&
				compare.ValidBet.ToDecimal() == source.ValidBet &&
				compare.Settle.ToDecimal() == source.Settle &&
				compare.Commission.ToDecimal() == source.Commission &&
				compare.Turnover.ToDecimal() == source.Turnover &&
				compare.WinLoss.ToDecimal() == source.WinLoss &&
				compare.Jackpot.ToDecimal() == source.Jackpot &&
				compare.CreatedOn == source.CreatedOn.ToTimestamp() &&
				compare.ClosedOn == source.ClosedOn.ToTimestamp() &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBetsWorkerNatsConst.GameBetRunningWorkerRequest),
			data: Arg.Is<Common.Protos.GameBetRunningWorkerRequest>(compare =>

				compare.GameBetId.ToGuid() == source.GameBetId &&
				compare.GameProviderId.ToGuid() == source.GameProviderId &&
				compare.GameProviderConfigurationId.ToGuid() == source.GameProviderConfigurationId &&
				compare.GameId == source.GameId &&
				compare.ResellerId.ToGuid() == source.ResellerId &&
				compare.LicenseeId.ToGuid() == source.LicenseeId &&
				compare.BrandId.ToGuid() == source.BrandId &&
				compare.PlayerCode == source.PlayerCode &&
				compare.CurrencyCode == source.CurrencyCode &&
				compare.WalletId.ToGuid() == source.WalletId &&
				compare.ExternalRoundId == source.ExternalRoundId &&
				compare.ExternalBetId == source.ExternalBetId &&
				compare.ExternalCreatedOn == source.ExternalCreatedOn.ToTimestamp() &&
				compare.BetState == source.BetState &&
				compare.BetType == source.BetType &&
				compare.CurrencyRatio == source.CurrencyRatio &&
				compare.Bet.ToDecimal() == source.Bet &&
				compare.ValidBet.ToDecimal() == source.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest),
			data: Arg.Is<Common.Protos.GameBetWorkerExpiryRequest>(compare =>

				compare.GameProviderId.ToGuid() == source.GameProviderId &&
				compare.GameId == source.GameId &&
				compare.BrandId.ToGuid() == source.BrandId),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
