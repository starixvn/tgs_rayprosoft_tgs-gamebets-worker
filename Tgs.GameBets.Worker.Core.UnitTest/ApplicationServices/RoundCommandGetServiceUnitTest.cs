using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Queue.Common.Enums;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class RoundCommandGetServiceUnitTest
{
	[Fact]
	public async Task Add_Round()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Running,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();

		_ = fakeRoundRepository.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundId);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.RoundAddModel(
					Id: roundId,
					GameProviderId: query.GameProviderId,
					ExternalRoundId: query.ExternalBetId,
					GameId: query.GameId,
					PlayerCode: query.PlayerCode,
					WalletId: query.WalletId,
					CurrencyCode: query.CurrencyCode,
					BrandId: query.BrandId,
					GameProviderConfigurationId: query.GameProviderConfigurationId,
					Status: RoundStatus.Open,
					ClientStatus: RoundClientStatus.Open,
					ExternalCreatedOn: externalCreatedOn,
					ExternalClosedOn: DateTimeOffset.UnixEpoch,
					Bet: query.Bet,
					ValidBet: query.ValidBet,
					Settle: 0.0m,
					Commission: 0.0m,
					Turnover: 0.0m,
					PlayerWinLoss: 0.0m)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetState: BetState.Win,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfoModel(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			ExternalRoundId: query.ExternalBetId,
			GameId: query.GameId,
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			Status: RoundStatus.Open,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			ClientStatus: RoundClientStatus.Open,
			ReopenedOn: null);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundClosedModel(
				ClientStatus: RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: query.Bet,
				ValidBet: query.ValidBet,
				Settle: query.Settle,
				Commission: query.Commission,
				Turnover: query.Turnover,
				PlayerWinLoss: query.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed_Get_DataBase_RoundStatus_Is_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetState: BetState.Win,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfoModel(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			ExternalRoundId: query.ExternalBetId,
			GameId: query.GameId,
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			Status: RoundStatus.Closed,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			ClientStatus: RoundClientStatus.Win,
			ReopenedOn: null);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Open()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetState: BetState.Running,
			Bet: 20.0m,
			ValidBet: 20.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfoModel(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			ExternalRoundId: query.ExternalBetId,
			GameId: query.GameId,
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			Status: RoundStatus.Open,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			ClientStatus: RoundClientStatus.Open,
			ReopenedOn: null);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundOpenModel(
				Bet: query.Bet,
				ValidBet: query.ValidBet)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetState: BetState.Win,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfoModel(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			ExternalRoundId: query.ExternalBetId,
			GameId: query.GameId,
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			Status: RoundStatus.Closed,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			ClientStatus: RoundClientStatus.Loss,
			ReopenedOn: null);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundReopenModel(
				ClientStatus: RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: query.Bet,
				ValidBet: query.ValidBet,
				Settle: query.Settle,
				Commission: query.Commission,
				Turnover: query.Turnover,
				PlayerWinLoss: query.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen_Is_RoundStatus_Open()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalBetId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UtcNow;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetState: BetState.Running,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalBetId"] = externalBetId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalBetId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfoModel(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			ExternalRoundId: query.ExternalBetId,
			GameId: query.GameId,
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			Status: RoundStatus.Closed,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			ClientStatus: RoundClientStatus.Win,
			ReopenedOn: null);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosedModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopenModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
