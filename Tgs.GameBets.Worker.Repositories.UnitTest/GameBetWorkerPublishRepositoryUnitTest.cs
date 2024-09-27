using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.Common.ViewModels;
using Tgs.GameBets.Worker.Core.Models;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Repositories.UnitTest;

public class GameBetWorkerPublishRepositoryUnitTest
{
	[Fact]
	public async Task GameBet_Command_Result_Is_Add()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishRepository>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new GameBetWorkerPublish(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			Result: GameBets.Common.Enums.GameBetCommandResult.Add);

		var sut = new GameBetWorkerPublishRepository(
			fakeLog,
			fakeMessageSender);

		await sut.PublishAsync(source);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
				UpdatedOn = source.ClosedOn.ToTimestamp(),
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
		var fakeLog = NullLogger<GameBetWorkerPublishRepository>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new GameBetWorkerPublish(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			Result: GameBets.Common.Enums.GameBetCommandResult.Update);

		var sut = new GameBetWorkerPublishRepository(
			fakeLog,
			fakeMessageSender);

		await sut.PublishAsync(source);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameBet_Command_Result_Is_None()
	{
		var fakeLog = NullLogger<GameBetWorkerPublishRepository>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var source = new GameBetWorkerPublish(
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
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			Result: GameBets.Common.Enums.GameBetCommandResult.None);

		var sut = new GameBetWorkerPublishRepository(
			fakeLog,
			fakeMessageSender);

		await sut.PublishAsync(source);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish),
			data: Arg.Is(new Common.Protos.GameBetsWorkerPublish
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
		var fakeLog = NullLogger<GameBetWorkerPublishRepository>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var source = new GameBetWorkerPublish(
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
			ExternalRoundId: externalCreatedOn.ToUnixTimeMilliseconds().ToString(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100.0m,
			ValidBet: 50.0m,
			Settle: 97.5m,
			WinLoss: 47.5m,
			Turnover: 50.0m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UnixEpoch,
			Result: GameBets.Common.Enums.GameBetCommandResult.Running);

		var sut = new GameBetWorkerPublishRepository(
			fakeLog,
			fakeMessageSender);

		await sut.PublishAsync(source);

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
