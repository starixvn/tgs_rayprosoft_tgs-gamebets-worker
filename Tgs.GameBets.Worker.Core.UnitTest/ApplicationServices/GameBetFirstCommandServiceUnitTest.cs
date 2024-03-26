using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetFirstCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_wager_adds_data()
	{
		var fakeGameBetFirstRepositoryRepository = Substitute.For<IGameBetFirstRepository>();
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeService = Substitute.For<ICommandService<GameWagerStatCloseCommand>>();

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

		_ = fakeGameBetFirstRepositoryRepository.ExistAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

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

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetFirstCommandService(
			fakeGameBetFirstRepositoryRepository,
			fakeGameBetRepositoryRepository,
			fakeSerialNumberGenerator,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetFirstRepositoryRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new GameBetFirstAddModel(
				Id: id,
				GameBetId: model.Id,
				GameProviderId: model.GameProviderId,
				GameProviderConfigurationId: model.GameProviderConfigurationId,
				GameId: model.GameId,
				ResellerId: model.ResellerId,
				LicenseeId: model.LicenseeId,
				BrandId: model.BrandId,
				PlayerCode: model.PlayerCode,
				CurrencyCode: model.CurrencyCode,
				WalletId: model.WalletId,
				ExternalRoundId: model.ExternalRoundId,
				ExternalBetId: model.ExternalBetId,
				ExternalCreatedOn: model.ExternalCreatedOn,
				ExternalClosedOn: model.ExternalClosedOn,
				ExternalPayoffOn: model.ExternalPayoffOn,
				ExternalResettleOn: model.ExternalResettleOn,
				BetState: model.BetState,
				BetType: model.BetType,
				FreeGameState: model.FreeGameState,
				GambleState: model.GambleState,
				CashoutState: model.CashoutState,
				ResettleState: model.ResettleState,
				JackpotState: model.JackpotState,
				CurrencyRatio: model.CurrencyRatio,
				Bet: model.Bet,
				ValidBet: model.ValidBet,
				Settle: model.Settle,
				Commission: model.Commission,
				Turnover: model.Turnover,
				WinLoss: model.WinLoss,
				Jackpot: model.Jackpot,
				CreatedOn: model.CreatedOn,
				ClosedOn: model.ClosedOn,
				UpdatedOn: model.UpdatedOn,
				AdditionalData: model.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: new GameWagerStatCloseCommand(
				GameProviderId: model.GameProviderId,
				GameId: model.GameId,
				PlayerCode: model.PlayerCode,
				WalletId: model.WalletId,
				ExternalRoundId: model.ExternalRoundId,
				ExternalBetId: model.ExternalBetId,
				ExternalCreatedOn: model.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_wager_information_already_exists()
	{
		var fakeGameBetFirstRepositoryRepository = Substitute.For<IGameBetFirstRepository>();
		var fakeGameBetRepositoryRepository = Substitute.For<IGameBetRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeService = Substitute.For<ICommandService<GameWagerStatCloseCommand>>();

		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameBetFirstCommand(
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			ExternalBetId: externalBetId,
			ExternalCreatedOn: externalCreatedOn,
			BetState: Queue.Common.Enums.BetState.Win);

		_ = fakeGameBetFirstRepositoryRepository.ExistAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new GameBetFirstCommandService(
			fakeGameBetFirstRepositoryRepository,
			fakeGameBetRepositoryRepository,
			fakeSerialNumberGenerator,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetFirstRepositoryRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetFirstAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
