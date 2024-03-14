using NSubstitute;
using Tgs.GameWagers.Common.Enums;
using Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameWagers.Settlement.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerSettlementFirstCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_wager_adds_data()
	{
		var fakeGameWagerSettlementFirstRepository = Substitute.For<IGameWagerSettlementFirstRepository>();
		var fakeGameWagerSettlementRepository = Substitute.For<IGameWagerSettlementRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameWagerSettlementFirstCommand(
			GameWagerPrimaryId: Guid.NewGuid(),
			ExternalCreatedOn: externalCreatedOn);

		_ = fakeGameWagerSettlementFirstRepository.ExistAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var model = new GameWagerSettlementInfoModel(
			Id: Guid.NewGuid(),
			GameWagerPrimaryId: command.GameWagerPrimaryId,
			WagerState: WagerState.Win,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50m,
			Settle: 97.5m,
			Commission: 0.0m,
			Turnover: 50,
			WinLoss: 47.5m,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow);

		_ = fakeGameWagerSettlementRepository.GetAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameWagerSettlementFirstCommandService(
			fakeGameWagerSettlementFirstRepository,
			fakeGameWagerSettlementRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerSettlementFirstRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new GameWagerSettlementFirstAddModel(
				Id: id,
				GameWagerPrimaryId: model.GameWagerPrimaryId,
				WagerState: model.WagerState,
				WagerType: model.WagerType,
				FreeGameState: model.FreeGameState,
				GambleState: model.GambleState,
				CashoutState: model.CashoutState,
				ResettleState: model.ResettleState,
				CurrencyRatio: model.CurrencyRatio,
				Bet: model.Bet,
				ValidBet: model.ValidBet,
				Settle: model.Settle,
				Commission: model.Commission,
				Turnover: model.Turnover,
				WinLoss: model.WinLoss,
				ExternalCreatedOn: model.ExternalCreatedOn,
				ExternalClosedOn: model.ExternalClosedOn,
				ExternalPayoffOn: model.ExternalPayoffOn,
				ExternalResettleOn: model.ExternalResettleOn,
				CreatedOn: model.CreatedOn,
				ClosedOn: model.ClosedOn,
				UpdatedOn: model.UpdatedOn)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_wager_information_already_exists()
	{
		var fakeGameWagerSettlementFirstRepository = Substitute.For<IGameWagerSettlementFirstRepository>();
		var fakeGameWagerSettlementRepository = Substitute.For<IGameWagerSettlementRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);

		var command = new GameWagerSettlementFirstCommand(
			GameWagerPrimaryId: Guid.NewGuid(),
			ExternalCreatedOn: externalCreatedOn);

		_ = fakeGameWagerSettlementFirstRepository.ExistAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new GameWagerSettlementFirstCommandService(
			fakeGameWagerSettlementFirstRepository,
			fakeGameWagerSettlementRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerSettlementFirstRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameWagerSettlementFirstAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
