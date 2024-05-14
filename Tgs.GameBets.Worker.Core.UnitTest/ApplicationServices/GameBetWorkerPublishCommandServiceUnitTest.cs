using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetWorkerPublishCommandServiceUnitTest
{
	[Fact]
	public async Task GameBet_Command_Result_Is_Add()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			Result: GameBets.Common.Enums.GameBetCommandResult.Add);

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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetWorkerPublishCommandService(
			fakeLog,
			fakeRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBet_Command_Result_Is_Update()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			Result: GameBets.Common.Enums.GameBetCommandResult.Update);

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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetWorkerPublishCommandService(
			fakeLog,
			fakeRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBet_Command_Result_Is_None()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			Result: GameBets.Common.Enums.GameBetCommandResult.None);

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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			});

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetWorkerPublishCommandService(
			fakeLog,
			fakeRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBet_Command_Result_Is_Running()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			Result: GameBets.Common.Enums.GameBetCommandResult.Running);

		var sut = new GameBetWorkerPublishCommandService(
			fakeLog,
			fakeRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(0)
			.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish),
			data: Arg.Any<Common.Protos.GameBetsWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
