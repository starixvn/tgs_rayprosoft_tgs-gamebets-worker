using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetRunningWorkerHandlerUnitTest
{
	[Fact]
	public async Task BetState_Is_Waiting()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Waiting,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(1)
			.CommandAsync(
			source: Arg.Is<GameBetRunningCommand>(compare =>
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
				compare.BetType == request.BetType &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Running,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(1)
			.CommandAsync(
			source: Arg.Is<GameBetRunningCommand>(compare =>
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
				compare.BetType == request.BetType &&
				compare.CurrencyRatio == request.CurrencyRatio &&
				compare.Bet == request.Bet &&
				compare.ValidBet == request.ValidBet &&
				compare.AdditionalData != null &&
				compare.ExtendedData != null),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Void()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Void,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Refund()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Refund,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Cancel()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Cancel,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Loss()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Win()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Draw()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.Draw,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_HalfWin()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.HalfWin,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_HalfLoss()
	{
		var fakeLog = NullLogger<GameBetRunningWorkerHandler>.Instance;
		var fakeRepository = Substitute.For<IGameBetRunningRepository>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var request = new GameBetRunningWorkerRequest(
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
			BetState: GameBets.Common.Enums.BetState.HalfLoss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			CurrencyRatio: 1.0m,
			Bet: 100.0m,
			ValidBet: 100.0m,
			CreatedOn: externalCreatedOn,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalBetId"] = externalBetId
			},
			ExtendedData: new JsonObject { });

		var sut = new GameBetRunningWorkerHandler(
			fakeLog,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(0)
			.CommandAsync(
			source: Arg.Any<GameBetRunningCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.DeleteAsync(
			gameBetId: Arg.Is(request.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
