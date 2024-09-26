using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetWorkerHandler(
	ILogger<GameBetWorkerHandler> logger,
	IGameBetRepository repository,
	IGameBetWorkerPublishRepository publishRepository) : IRequestHandler<GameBetWorkerRequest>
{
	public async Task Handle(GameBetWorkerRequest request, CancellationToken cancellationToken)
	{
		if (request.BetState is GameBets.Common.Enums.BetState.Waiting or GameBets.Common.Enums.BetState.Running)
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						request.GameProviderId,
						request.GameId,
						request.PlayerCode,
						request.WalletId,
						request.ExternalRoundId,
						request.ExternalBetId,
						ExternalCreatedOn = request.ExternalCreatedOn.ToString("u"),
						request.BetState
					},
					Message = "The bet status of this game is not settled."
				}));
			return;
		}

		var gameBetCommandResult = await repository.CommandAsync(
			source: new GameBetCommandModel(
				GameBetId: request.GameBetId,
				GameProviderId: request.GameProviderId,
				GameProviderConfigurationId: request.GameProviderConfigurationId,
				GameId: request.GameId,
				ResellerId: request.ResellerId,
				LicenseeId: request.LicenseeId,
				BrandId: request.BrandId,
				PlayerCode: request.PlayerCode,
				CurrencyCode: request.CurrencyCode,
				WalletId: request.WalletId,
				ExternalRoundId: request.ExternalRoundId,
				ExternalBetId: request.ExternalBetId,
				ExternalCreatedOn: request.ExternalCreatedOn,
				ExternalClosedOn: request.ExternalClosedOn,
				ExternalPayoffOn: request.ExternalPayoffOn,
				ExternalResettleOn: request.ExternalResettleOn,
				BetState: request.BetState,
				BetType: request.BetType,
				FreeGameState: request.FreeGameState,
				GambleState: request.GambleState,
				CashoutState: request.CashoutState,
				ResettleState: request.ResettleState,
				JackpotState: request.JackpotState,
				CurrencyRatio: request.CurrencyRatio,
				Bet: request.Bet,
				ValidBet: request.ValidBet,
				Settle: request.Settle,
				Commission: request.Commission,
				Turnover: request.Turnover,
				WinLoss: request.WinLoss,
				Jackpot: request.Jackpot,
				AdditionalData: request.AdditionalData),
			cancellationToken)
			.ConfigureAwait(false);

		await publishRepository.PublishAsync(
			source: new GameBetWorkerPublish(
				GameProviderId: request.GameProviderId,
				GameId: request.GameId,
				PlayerCode: request.PlayerCode,
				WalletId: request.WalletId,
				ExternalBetId: request.ExternalBetId,
				ExternalCreatedOn: request.ExternalCreatedOn,
				Result: gameBetCommandResult),
			cancellationToken)
			.ConfigureAwait(false);
	}
}
