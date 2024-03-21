using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common.Enums;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;
public class GameBetWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task This_marker_does_not_exist_Add_it()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameBetMarkerRepository = Substitute.For<IGameBetMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeGameBetHandlerService = Substitute.For<ICommandService<GameBetHandlerCommand>>();

		var command = new GameBetWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameWagerMarkerInfo = new GameBetMarkerInfoModel(
			Code: command.GameProviderCode,
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " Game Bet Worker"));
		_ = fakeGameBetMarkerRepository.GetAsync(
			Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(null, gameWagerMarkerInfo);

		_ = fakeGameGameWagersClient.GameBetsEventSourceQueryAllAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Is(gameWagerMarkerInfo.Marker),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<GameWagers.Common.GameBetsEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameBetWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameBetMarkerRepository,
			fakeGameGameWagersClient,
			fakeGameBetHandlerService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetMarkerRepository
			.Received(1)
			.AddAsync(
			code: Arg.Is(command.GameProviderCode),
			directions: string.Concat(command.GameProviderCode, " Game Bet Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetHandlerService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetHandlerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetMarkerRepository
			.Received(1)
			.UpdateAsync(
			code: Arg.Is(command.GameProviderCode),
			marker: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_event_source_has_no_data()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameBetMarkerRepository = Substitute.For<IGameBetMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeGameBetHandlerService = Substitute.For<ICommandService<GameBetHandlerCommand>>();

		var command = new GameBetWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameBetMarkerInfo = new GameBetMarkerInfoModel(
			Code: command.GameProviderCode,
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " Game Bet Worker"));
		_ = fakeGameBetMarkerRepository.GetAsync(
			Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetMarkerInfo);

		_ = fakeGameGameWagersClient.GameBetsEventSourceQueryAllAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Is(gameBetMarkerInfo.Marker),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<GameWagers.Common.GameBetsEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameBetWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameBetMarkerRepository,
			fakeGameGameWagersClient,
			fakeGameBetHandlerService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetMarkerRepository
			.Received(0)
			.AddAsync(
			code: Arg.Is(command.GameProviderCode),
			directions: string.Concat(command.GameProviderCode, " Game Bet Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetHandlerService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetHandlerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetMarkerRepository
			.Received(1)
			.UpdateAsync(
			code: Arg.Is(command.GameProviderCode),
			marker: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_event_has_data()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameBetMarkerRepository = Substitute.For<IGameBetMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeGameBetHandlerService = Substitute.For<ICommandService<GameBetHandlerCommand>>();

		var command = new GameBetWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameWagerMarkerInfo = new GameBetMarkerInfoModel(
			Code: command.GameProviderCode,
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " Game Bet Worker"));
		_ = fakeGameBetMarkerRepository.GetAsync(
			Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerMarkerInfo);

		var externalRoundId = DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeMilliseconds().ToString();
		var externalWagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var gameBetsEventSourceInfo = new GameWagers.Common.GameBetsEventSourceInfo(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			ExternalWagerId: externalWagerId,
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
				["WagerId"] = externalWagerId,
				["BetOn"] = externalCreatedOn,
				["SettleOn"] = externalClosedOn,
				["PayoffOn"] = externalPayoffOn
			});
		_ = fakeGameGameWagersClient.GameBetsEventSourceQueryAllAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Any<Guid>(),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { gameBetsEventSourceInfo }.ToAsyncEnumerable(), Enumerable.Empty<GameWagers.Common.GameBetsEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameBetWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameBetMarkerRepository,
			fakeGameGameWagersClient,
			fakeGameBetHandlerService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetMarkerRepository
			.Received(0)
			.AddAsync(
			code: Arg.Is(command.GameProviderCode),
			directions: string.Concat(command.GameProviderCode, " Game Bet Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetHandlerService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetHandlerCommand(
				GameProviderId: gameBetsEventSourceInfo.GameProviderId,
				GameProviderConfigurationId: gameBetsEventSourceInfo.GameProviderConfigurationId,
				GameId: gameBetsEventSourceInfo.GameId,
				ResellerId: gameBetsEventSourceInfo.ResellerId,
				LicenseeId: gameBetsEventSourceInfo.LicenseeId,
				BrandId: gameBetsEventSourceInfo.BrandId,
				PlayerCode: gameBetsEventSourceInfo.PlayerCode,
				CurrencyCode: gameBetsEventSourceInfo.CurrencyCode,
				WalletId: gameBetsEventSourceInfo.WalletId,
				ExternalRoundId: gameBetsEventSourceInfo.ExternalRoundId,
				ExternalWagerId: gameBetsEventSourceInfo.ExternalWagerId,
				ExternalCreatedOn: gameBetsEventSourceInfo.ExternalCreatedOn,
				ExternalClosedOn: gameBetsEventSourceInfo.ExternalClosedOn,
				ExternalPayoffOn: gameBetsEventSourceInfo.ExternalPayoffOn,
				ExternalResettleOn: gameBetsEventSourceInfo.ExternalResettleOn,
				BetState: gameBetsEventSourceInfo.BetState,
				BetType: gameBetsEventSourceInfo.BetType,
				FreeGameState: gameBetsEventSourceInfo.FreeGameState,
				GambleState: gameBetsEventSourceInfo.GambleState,
				CashoutState: gameBetsEventSourceInfo.CashoutState,
				ResettleState: gameBetsEventSourceInfo.ResettleState,
				JackpotState: gameBetsEventSourceInfo.JackpotState,
				CurrencyRatio: gameBetsEventSourceInfo.CurrencyRatio,
				Bet: gameBetsEventSourceInfo.Bet,
				ValidBet: gameBetsEventSourceInfo.ValidBet,
				Settle: gameBetsEventSourceInfo.Settle,
				Commission: gameBetsEventSourceInfo.Commission,
				Turnover: gameBetsEventSourceInfo.Turnover,
				WinLoss: gameBetsEventSourceInfo.WinLoss,
				Jackpot: gameBetsEventSourceInfo.Jackpot,
				AdditionalData: gameBetsEventSourceInfo.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameBetMarkerRepository
			.Received(2)
			.UpdateAsync(
			code: Arg.Is(command.GameProviderCode),
			marker: Arg.Is(gameBetsEventSourceInfo.Id),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
