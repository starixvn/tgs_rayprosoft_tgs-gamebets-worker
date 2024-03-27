using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.Models;

using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetCommandService(
	ILogger<GameBetCommandService> logger,
	IGameBetRepository repository) : ICommandService<GameBetCommand>
{
	public async ValueTask ExecuteAsync(GameBetCommand command, CancellationToken cancellationToken = default)
	{
		var model = await repository.GetAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is null)
		{
			await repository.AddAsync(
				source: new GameBetAddModel(
					Id: command.GameBetId,
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
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			if (
				model.BetState == command.BetState &&
				model.BetType == command.BetType &&
				model.FreeGameState == command.FreeGameState &&
				model.GambleState == command.GambleState &&
				model.CashoutState == command.CashoutState &&
				model.ResettleState == command.ResettleState &&
				model.JackpotState == command.JackpotState &&
				model.Bet == command.Bet &&
				model.ValidBet == command.ValidBet &&
				model.Settle == command.Settle &&
				model.Commission == command.Commission &&
				model.Turnover == command.Turnover &&
				model.WinLoss == command.WinLoss &&
				model.Jackpot == command.Jackpot)
			{
				return;
			}

			logger.LogWarning("LogOn:{logOn} Message:The content of this Game Wager order is different, please change the Game Wager.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					model.Id,
					command.GameProviderId,
					command.GameId,
					command.PlayerCode,
					command.WalletId,
					command.ExternalBetId,
				});

			await repository.UpdateAsync(
				source: new GameBetUpdateModel(
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
					AdditionalData: command.AdditionalData),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
