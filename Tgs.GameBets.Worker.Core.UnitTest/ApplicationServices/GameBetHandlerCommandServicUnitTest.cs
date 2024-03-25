using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Queue.Common.Enums;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetHandlerCommandServicUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRoundService = Substitute.For<IGetService<RoundCommand, Guid>>();
		var fakeGameBetService = Substitute.For<ICommandService<GameBetCommand>>();
		var fakeGameBetFirstService = Substitute.For<ICommandService<GameBetFirstCommand>>();

		var externalRoundId = DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeMilliseconds().ToString();
		var externalBetId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetHandlerCommand(
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
				["BetId"] = externalBetId,
				["BetOn"] = externalCreatedOn,
				["SettleOn"] = externalClosedOn,
				["PayoffOn"] = externalPayoffOn
			});

		var id = Guid.NewGuid();

		_ = fakeRoundService.GetAsync(
			query: Arg.Is(new RoundCommand(
				GameProviderId: command.GameProviderId,
				GameProviderConfigurationId: command.GameProviderConfigurationId,
				GameId: command.GameId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				CurrencyCode: command.CurrencyCode,
				WalletId: command.WalletId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				BetState: command.BetState,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				PlayerWinLoss: command.WinLoss,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new GameBetHandlerCommandServic(
			fakeRoundService,
			fakeGameBetService,
			fakeGameBetFirstService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetCommand(
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
				ExternalBetId: command.ExternalBetId,
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

		_ = fakeGameBetFirstService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetFirstCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				BetState: command.BetState)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
