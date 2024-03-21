using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameWagers.Common.Enums;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;
public class GameBetCommandServiceUnitTest
{
	[Fact]
	public async Task This_game_wager_adds_data()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalWagerId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
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
				["ExternalRoundId"] = externalRoundId,
				["ExternalWagerId"] = externalWagerId
			});

		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalWagerId: Arg.Is(command.ExternalWagerId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameBetInfoModel?)null);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new GameBetAddModel(
					Id: id,
					GameProviderId: command.GameProviderId,
					GameProviderConfigurationId: command.GameProviderConfigurationId,
					GameId: command.GameId,
					ResellerId: command.ResellerId,
					LicenseeId: command.LicenseeId,
					BrandId: command.BrandId,
					PlayerCode: command.PlayerCode,
					CurrencyCode: command.CurrencyCode,
					WalletId: command.WalletId,
					ExternalRoundId: command.ExternalRoundId,
					ExternalWagerId: command.ExternalWagerId,
					ExternalCreatedOn: command.ExternalCreatedOn,
					ExternalClosedOn: command.ExternalClosedOn,
					ExternalPayoffOn: command.ExternalPayoffOn,
					ExternalResettleOn: command.ExternalResettleOn,
					BetState: command.BetState,
					BetType: command.BetType,
					FreeGameState: command.FreeGameState,
					GambleState: command.GambleState,
					CashoutState: command.CashoutState,
					ResettleState: command.ResettleState,
					JackpotState: command.JackpotState,
					CurrencyRatio: command.CurrencyRatio,
					Bet: command.Bet,
					ValidBet: command.ValidBet,
					Settle: command.Settle,
					Commission: command.Commission,
					Turnover: command.Turnover,
					WinLoss: command.WinLoss,
					Jackpot: command.Jackpot,
					AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_wager_update_data()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalWagerId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
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
				["ExternalRoundId"] = externalRoundId,
				["ExternalWagerId"] = externalWagerId
			});

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
			ExternalWagerId: externalWagerId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Lose,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 0.0m,
			Commission: 0,
			Turnover: 50,
			WinLoss: -50m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalWagerId"] = externalWagerId
			});
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalWagerId: Arg.Is(command.ExternalWagerId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateAsync(
			source: Arg.Is(new GameBetUpdateModel(
				Id: model.Id,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalResettleOn: command.ExternalResettleOn,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				CurrencyRatio: command.CurrencyRatio,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss,
				Jackpot: command.Jackpot,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_betting_information_does_not_need_to_be_changed()
	{
		var fakeLog = NullLogger<GameBetCommandService>.Instance;
		var fakeRepository = Substitute.For<IGameBetRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalWagerId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetCommand(
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
				["ExternalRoundId"] = externalRoundId,
				["ExternalWagerId"] = externalWagerId
			});

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
			CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-5),
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId,
				["ExternalWagerId"] = externalWagerId
			});
		_ = fakeRepository.GetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalWagerId: Arg.Is(command.ExternalWagerId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(model);

		var sut = new GameBetCommandService(
			fakeLog,
			fakeRepository,
			fakeSerialNumberGenerator);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<GameBetAddModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(0)
			.UpdateAsync(
			source: Arg.Any<GameBetUpdateModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
