using System.Text.Json.Nodes;
using System.Threading;
using NSubstitute;
using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common;
using Tgs.GameWagers.Common.Enums;
using Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Settlement.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerSettlementWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task This_marker_does_not_exist_Add_it()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameWagerMarkerRepository = Substitute.For<IGameWagerMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeSettlementService = Substitute.For<ICommandService<GameWagerSettlementCommand>>();
		var fakeSettlementFirstService = Substitute.For<ICommandService<GameWagerSettlementFirstCommand>>();

		var command = new GameWagerSettlementWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameWagerMarkerInfo = new GameWagerMarkerInfoModel(
			Code: string.Concat(command.GameProviderCode, "-", "settlement").ToUpper(),
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"));
		_ = fakeGameWagerMarkerRepository.GetAsync(
			Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(null, gameWagerMarkerInfo);

		_ = fakeGameGameWagersClient.GameWagersEventSourceQuerySettlementsAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Is(gameWagerMarkerInfo.Marker),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<GameWagersEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameWagerSettlementWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameWagerMarkerRepository,
			fakeGameGameWagersClient,
			fakeSettlementService,
			fakeSettlementFirstService);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerMarkerRepository
			.Received(1)
			.AddAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerSettlementCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementFirstService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerSettlementFirstCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerMarkerRepository
			.Received(1)
			.UpdateAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			marker: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_event_source_has_no_data()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameWagerMarkerRepository = Substitute.For<IGameWagerMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeSettlementService = Substitute.For<ICommandService<GameWagerSettlementCommand>>();
		var fakeSettlementFirstService = Substitute.For<ICommandService<GameWagerSettlementFirstCommand>>();

		var command = new GameWagerSettlementWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameWagerMarkerInfo = new GameWagerMarkerInfoModel(
			Code: string.Concat(command.GameProviderCode, "-", "settlement").ToUpper(),
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"));
		_ = fakeGameWagerMarkerRepository.GetAsync(
			Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerMarkerInfo);

		_ = fakeGameGameWagersClient.GameWagersEventSourceQuerySettlementsAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Is(gameWagerMarkerInfo.Marker),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<GameWagersEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameWagerSettlementWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameWagerMarkerRepository,
			fakeGameGameWagersClient,
			fakeSettlementService,
			fakeSettlementFirstService);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerMarkerRepository
			.Received(0)
			.AddAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerSettlementCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementFirstService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerSettlementFirstCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerMarkerRepository
			.Received(1)
			.UpdateAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			marker: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_event_has_data()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameWagerMarkerRepository = Substitute.For<IGameWagerMarkerRepository>();
		var fakeGameGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeSettlementService = Substitute.For<ICommandService<GameWagerSettlementCommand>>();
		var fakeSettlementFirstService = Substitute.For<ICommandService<GameWagerSettlementFirstCommand>>();

		var command = new GameWagerSettlementWorkerCommand(
			"KY");

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameWagerMarkerInfo = new GameWagerMarkerInfoModel(
			Code: string.Concat(command.GameProviderCode, "-", "settlement").ToUpper(),
			Marker: Guid.Empty,
			Expansions: new JsonObject().ToJsonString(),
			Directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"));
		_ = fakeGameWagerMarkerRepository.GetAsync(
			Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerMarkerInfo);

		var externalRoundId = DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeMilliseconds().ToString();
		var externalWagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;
		var gameWagersEventSourceInfo = new GameWagersEventSourceInfo(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
			GameWagerPrimaryId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			WagerState: WagerState.Win,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			ExternalRoundId: externalRoundId,
			ExternalWagerId: externalWagerId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 97.5m,
			Commission: 0,
			Turnover: 50,
			WinLoss: 47.5m,
			AdditionalData: new JsonObject
			{
				["RoundId"] = externalRoundId,
				["WagerId"] = externalWagerId,
				["BetOn"] = externalCreatedOn,
				["SettleOn"] = externalClosedOn,
				["PayoffOn"] = externalPayoffOn
			});
		_ = fakeGameGameWagersClient.GameWagersEventSourceQuerySettlementsAsync(
			gameProviderId: Arg.Is(gameProviderId),
			id: Arg.Any<Guid>(),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { gameWagersEventSourceInfo }.ToAsyncEnumerable(), Enumerable.Empty<GameWagersEventSourceInfo>().ToAsyncEnumerable());

		var sut = new GameWagerSettlementWorkerCommandService(
			fakeGameProviderRepository,
			fakeGameWagerMarkerRepository,
			fakeGameGameWagersClient,
			fakeSettlementService,
			fakeSettlementFirstService);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerMarkerRepository
			.Received(0)
			.AddAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			directions: string.Concat(command.GameProviderCode, " GameWagers Settlement Worker"),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerSettlementCommand(
				GameWagerPrimaryId: gameWagersEventSourceInfo.GameWagerPrimaryId,
				WagerState: gameWagersEventSourceInfo.WagerState,
				WagerType: gameWagersEventSourceInfo.WagerType,
				FreeGameState: gameWagersEventSourceInfo.FreeGameState,
				GambleState: gameWagersEventSourceInfo.GambleState,
				CashoutState: gameWagersEventSourceInfo.CashoutState,
				ResettleState: gameWagersEventSourceInfo.ResettleState,
				CurrencyRatio: gameWagersEventSourceInfo.CurrencyRatio,
				Bet: gameWagersEventSourceInfo.Bet,
				ValidBet: gameWagersEventSourceInfo.ValidBet,
				Settle: gameWagersEventSourceInfo.Settle,
				Commission: gameWagersEventSourceInfo.Commission,
				Turnover: gameWagersEventSourceInfo.Turnover,
				WinLoss: gameWagersEventSourceInfo.WinLoss,
				ExternalCreatedOn: gameWagersEventSourceInfo.ExternalCreatedOn,
				ExternalClosedOn: gameWagersEventSourceInfo.ExternalClosedOn,
				ExternalPayoffOn: gameWagersEventSourceInfo.ExternalPayoffOn,
				ExternalResettleOn: gameWagersEventSourceInfo.ExternalResettleOn)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeSettlementFirstService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameWagerSettlementFirstCommand(
				GameWagerPrimaryId: gameWagersEventSourceInfo.GameWagerPrimaryId,
				ExternalCreatedOn: gameWagersEventSourceInfo.ExternalCreatedOn)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerMarkerRepository
			.Received(2)
			.UpdateAsync(
			code: Arg.Is(string.Concat(command.GameProviderCode, "-", "settlement").ToUpper()),
			marker: Arg.Is(gameWagersEventSourceInfo.Id),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
