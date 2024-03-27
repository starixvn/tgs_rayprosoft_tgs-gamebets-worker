using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.UnitTest.ApplicationServices;

public class GameBetWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetService = Substitute.For<ICommandService<GameBetCommand>>();
		var fakeGameBetFirstService = Substitute.For<ICommandService<GameBetFirstCommand>>();

		var externalRoundId = Guid.NewGuid().ToString();
		var externalBetId = Guid.NewGuid().ToString();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5);
		var externalClosedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalPayoffOn = DateTimeOffset.UtcNow;

		var command = new GameBetWorkerCommand(
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
			ExternalClosedOn: externalClosedOn,
			ExternalPayoffOn: externalPayoffOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
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
				["ExternalBetId"] = externalBetId
			});

		var sut = new GameBetWorkerCommandService(
			fakeGameBetService,
			fakeGameBetFirstService);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetCommand(
				GameBetId: command.GameBetId,
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

		_ = fakeGameBetFirstService.ExecuteAsync(
			command: Arg.Is(new GameBetFirstCommand(
			GameProviderId: command.GameProviderId,
			GameId: command.GameId,
			PlayerCode: command.PlayerCode,
			WalletId: command.WalletId,
			ExternalBetId: command.ExternalBetId,
			ExternalCreatedOn: command.ExternalCreatedOn)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
