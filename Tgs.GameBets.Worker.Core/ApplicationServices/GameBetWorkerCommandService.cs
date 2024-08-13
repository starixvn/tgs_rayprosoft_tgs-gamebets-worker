using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerCommandService(
	ILogger<GameBetWorkerCommandService> logger,
	IGameBetRepository repository,
	ICommandService<GameBetWorkerPublishCommand> gameBetWorkerPublishService) : ICommandService<GameBetWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		if (command.BetState is GameBets.Common.Enums.BetState.Waiting or GameBets.Common.Enums.BetState.Running)
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						command.GameProviderId,
						command.GameId,
						command.PlayerCode,
						command.WalletId,
						command.ExternalRoundId,
						command.ExternalBetId,
						ExternalCreatedOn = command.ExternalCreatedOn.ToString("u"),
						command.BetState
					},
					Message = "The bet status of this game is not settled."
				}));
			return;
		}

		var gameBetCommandResult = await repository.CommandAsync(
			source: new GameBetCommandModel(
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
				AdditionalData: command.AdditionalData),
			cancellationToken)
			.ConfigureAwait(false);

		await gameBetWorkerPublishService.ExecuteAsync(
			command: new GameBetWorkerPublishCommand(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalBetId: command.ExternalBetId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				Result: gameBetCommandResult),
			cancellationToken)
			.ConfigureAwait(false);
	}
}
