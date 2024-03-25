using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Queue.Client;
using Tgs.GameBets.Queue.Common.Enums;
using Tgs.GameBets.Queue.Common.ViewModels;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_betting_event_source_has_no_data()
	{
		var fakeGameBetQueueClient = Substitute.For<IGameBetQueueClient>();
		var fakeGameBetHandlerService = Substitute.For<ICommandService<GameBetHandlerCommand>>();

		var command = new GameBetWorkerCommand(
			"KY");

		_ = fakeGameBetQueueClient.PopAsync(
			gpCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetQueueInfoViewModel?)null);

		var sut = new GameBetWorkerCommandService(
			fakeGameBetQueueClient,
			fakeGameBetHandlerService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetHandlerService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetHandlerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetQueueClient
			.Received(0)
			.RemoveAsync(
			gpCode: Arg.Is(command.GameProviderCode),
			source: Arg.Any<GameBetQueueRemoveViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_event_has_data()
	{
		var fakeGameBetQueueClient = Substitute.For<IGameBetQueueClient>();
		var fakeGameBetHandlerService = Substitute.For<ICommandService<GameBetHandlerCommand>>();

		var command = new GameBetWorkerCommand(
			"KY");

		var externalRoundId = DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeMilliseconds().ToString();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var gameBetQueueInfo = new GameBetQueueInfoViewModel(
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
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
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
				["RoundId"] = externalRoundId,
				["WagerId"] = externalBetId,
				["BetOn"] = externalCreatedOn,
				["SettleOn"] = externalClosedOn,
				["PayoffOn"] = externalPayoffOn
			});
		_ = fakeGameBetQueueClient.PopAsync(
			gpCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetQueueInfo, (GameBetQueueInfoViewModel?)null);

		var sut = new GameBetWorkerCommandService(
			fakeGameBetQueueClient,
			fakeGameBetHandlerService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetHandlerService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetHandlerCommand(
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
				AdditionalData: gameBetQueueInfo.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetQueueClient.RemoveAsync(
			gpCode: Arg.Is(command.GameProviderCode),
			source: Arg.Is(new GameBetQueueRemoveViewModel
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
