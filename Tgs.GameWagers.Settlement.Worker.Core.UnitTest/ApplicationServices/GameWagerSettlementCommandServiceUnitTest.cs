using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using NSubstitute;
using Tgs.GameWagers.Common.Enums;
using Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameWagers.Settlement.Worker.Core.UnitTest.ApplicationServices;
public class GameWagerSettlementCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_wager_adds_data()
	{
		var fakeLog = NullLogger<GameWagerSettlementCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameWagerSettlementRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameWagerSettlementCommand(
			GameWagerPrimaryId: Guid.NewGuid(),
			WagerState: WagerState.Win,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
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
			WinLoss: 47.5m);

		_ = fakeRepository.GetAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameWagerSettlementInfoModel?)null);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameWagerSettlementCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new GameWagerSettlementAddModel(
				Id: id,
				GameWagerPrimaryId: command.GameWagerPrimaryId,
				WagerState: command.WagerState,
				WagerType: command.WagerType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				ExternalPayoffOn: command.ExternalPayoffOn,
				ExternalResettleOn: command.ExternalResettleOn)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameWagerSettlementUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_wager_update_data()
	{
		var fakeLog = NullLogger<GameWagerSettlementCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameWagerSettlementRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameWagerSettlementCommand(
			GameWagerPrimaryId: Guid.NewGuid(),
			WagerState: WagerState.Win,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
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
			WinLoss: 47.5m);

		var model = new GameWagerSettlementInfoModel(
			Id: Guid.NewGuid(),
			GameWagerPrimaryId: command.GameWagerPrimaryId,
			WagerState: WagerState.Lose,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 100m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 100m,
			WinLoss: -100m,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow);
		_ = fakeRepository.GetAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameWagerSettlementCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameWagerSettlementAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is(new GameWagerSettlementUpdateModel(
				Id: model.Id,
				ExternalCreatedOn: command.ExternalCreatedOn,
				WagerState: command.WagerState,
				WagerType: command.WagerType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				ExternalResettleOn: command.ExternalResettleOn)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameWagerSettlementCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameWagerSettlementRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameWagerSettlementCommand(
			GameWagerPrimaryId: Guid.NewGuid(),
			WagerState: WagerState.Win,
			WagerType: WagerType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
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
			WinLoss: 47.5m);

		var model = new GameWagerSettlementInfoModel(
			Id: Guid.NewGuid(),
			GameWagerPrimaryId: command.GameWagerPrimaryId,
			WagerState: command.WagerState,
			WagerType: command.WagerType,
			FreeGameState: command.FreeGameState,
			GambleState: command.GambleState,
			CashoutState: command.CashoutState,
			ResettleState: command.ResettleState,
			CurrencyRatio: command.CurrencyRatio,
			Bet: command.Bet,
			ValidBet: command.ValidBet,
			Settle: command.Settle,
			Commission: command.Commission,
			Turnover: command.Turnover,
			WinLoss: command.WinLoss,
			ExternalCreatedOn: command.ExternalCreatedOn,
			ExternalClosedOn: command.ExternalClosedOn,
			ExternalPayoffOn: command.ExternalPayoffOn,
			ExternalResettleOn: command.ExternalResettleOn,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow);
		_ = fakeRepository.GetAsync(
			gameWagerPrimaryId: Arg.Is(command.GameWagerPrimaryId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameWagerSettlementCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameWagerSettlementAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameWagerSettlementUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}