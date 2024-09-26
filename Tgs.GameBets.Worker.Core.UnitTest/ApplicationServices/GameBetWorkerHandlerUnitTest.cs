using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetWorkerHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<GameBetWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakePublishRepository = Substitute.For<IGameBetWorkerPublishRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var request = new GameBetWorkerRequest(
			GameBetId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeRepository.CommandAsync(
			source: Arg.Is<GameBetCommandModel>(compare =>
				compare.GameBetId == request.GameBetId &&
				compare.GameProviderId == request.GameProviderId &&
				compare.GameProviderConfigurationId == request.GameProviderConfigurationId &&
				compare.GameId == request.GameId &&
				compare.ResellerId == request.ResellerId &&
				compare.LicenseeId == request.LicenseeId &&
				compare.BrandId == request.BrandId &&
				compare.PlayerCode == request.PlayerCode &&
				compare.CurrencyCode == request.CurrencyCode &&
				compare.WalletId == request.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == request.BetState &&
				compare.BetType == request.BetType &&
				compare.FreeGameState == request.FreeGameState &&
				compare.GambleState == request.GambleState &&
				compare.CashoutState == request.CashoutState &&
				compare.ResettleState == request.ResettleState &&
				compare.JackpotState == request.JackpotState &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.Settle == request.Settle &&
				compare.WinLoss == request.WinLoss &&
				compare.Turnover == request.Turnover &&
				compare.Commission == request.Commission &&
				compare.Jackpot == request.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameBets.Common.Enums.GameBetCommandResult.Add);

		var sut = new GameBetWorkerHandler(
			fakeLog,
			fakeRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is(new GameBetWorkerPublish(
				GameProviderId: request.GameProviderId,
				GameId: request.GameId,
				PlayerCode: request.PlayerCode,
				WalletId: request.WalletId,
				ExternalBetId: request.ExternalBetId,
				ExternalCreatedOn: request.ExternalCreatedOn,
				Result: GameBets.Common.Enums.GameBetCommandResult.Add)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Waiting()
	{
		var fakeLog = NullLogger<GameBetWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakePublishRepository = Substitute.For<IGameBetWorkerPublishRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetWorkerRequest(
			GameBetId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Waiting,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var sut = new GameBetWorkerHandler(
			fakeLog,
			fakeRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetCommandModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<GameBetWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running()
	{
		var fakeLog = NullLogger<GameBetWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakePublishRepository = Substitute.For<IGameBetWorkerPublishRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetWorkerRequest(
			GameBetId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Running,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 100.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		var sut = new GameBetWorkerHandler(
			fakeLog,
			fakeRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetCommandModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<GameBetWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
