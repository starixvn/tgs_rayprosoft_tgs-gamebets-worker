using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameBets.First.Worker.Common.Protos;
using Tgs.GameBets.First.Worker.Common;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;
using Valhalla.MessageQueue;
using NSubstitute.ExceptionExtensions;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetFirstCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetFirstCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetState: Queue.Common.Enums.BetState.Win);

		var model = new GameBetInfoModel(
			Id: Guid.NewGuid(),
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
			BetState: Queue.Common.Enums.BetState.Win,
			BetType: Queue.Common.Enums.BetType.Normal,
			FreeGameState: Queue.Common.Enums.FreeGameState.None,
			GambleState: Queue.Common.Enums.GambleState.None,
			CashoutState: Queue.Common.Enums.CashoutState.None,
			ResettleState: Queue.Common.Enums.ResettleState.None,
			JackpotState: Queue.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeGameBetRepositoryRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetFirstCommandService(
			fakeGameBetRepositoryRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBetsFirstWorkerNatsConst.GameBetsFirstWorkerRequest),
			data: Arg.Is(new GameBetsFirstWorkerRequest
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
				BetState = (BetState)model.BetState,
				BetType = (BetType)model.BetType,
				FreeGameState = (FreeGameState)model.FreeGameState,
				GambleState = (GambleState)model.GambleState,
				CashoutState = (CashoutState)model.CashoutState,
				ResettleState = (ResettleState)model.ResettleState,
				JackpotState = (JackpotState)model.JackpotState,
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ArgumentNullException()
	{
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetFirstCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetState: Queue.Common.Enums.BetState.Win);
		_ = fakeGameBetRepositoryRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.ThrowsAsync(new ArgumentNullException());

		var sut = new GameBetFirstCommandService(
			fakeGameBetRepositoryRepository,
			fakeMessageSender);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(GameBetsFirstWorkerNatsConst.GameBetsFirstWorkerRequest),
			data: Arg.Any<GameBetsFirstWorkerRequest>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Waiting()
	{
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetFirstCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetState: Queue.Common.Enums.BetState.Waiting);

		var sut = new GameBetFirstCommandService(
			fakeGameBetRepositoryRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(GameBetsFirstWorkerNatsConst.GameBetsFirstWorkerRequest),
			data: Arg.Any<GameBetsFirstWorkerRequest>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running()
	{
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetFirstCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetState: Queue.Common.Enums.BetState.Waiting);

		var sut = new GameBetFirstCommandService(
			fakeGameBetRepositoryRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(GameBetsFirstWorkerNatsConst.GameBetsFirstWorkerRequest),
			data: Arg.Any<GameBetsFirstWorkerRequest>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
