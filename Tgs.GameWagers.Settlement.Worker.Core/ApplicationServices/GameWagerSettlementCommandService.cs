using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
internal class GameWagerSettlementCommandService(
	ILogger<GameWagerSettlementCommandService> logger,
	IGameWagerSettlementRepository repository,
	ISerialNumberGenerator serialNumberGenerator) : ICommandService<GameWagerSettlementCommand>
{
	public async ValueTask ExecuteAsync(GameWagerSettlementCommand command, CancellationToken cancellationToken = default)
	{
		var model = await repository.GetAsync(
			gameWagerPrimaryId: command.GameWagerPrimaryId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (model is null)
		{
			var id = await serialNumberGenerator.GenerateAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await repository.AddAsync(
				source: new GameWagerSettlementAddModel(
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
					ExternalResettleOn: command.ExternalResettleOn),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			if (
				model.WagerState == command.WagerState &&
				model.WagerType == command.WagerType &&
				model.FreeGameState == command.FreeGameState &&
				model.GambleState == command.GambleState &&
				model.CashoutState == command.CashoutState &&
				model.ResettleState == command.ResettleState &&
				model.Bet == command.Bet &&
				model.ValidBet == command.ValidBet &&
				model.Settle == command.Settle &&
				model.Commission == command.Commission &&
				model.Turnover == command.Turnover &&
				model.WinLoss == command.WinLoss)
			{
				return;
			}

			logger.LogWarning("LogOn:{logOn} Message:The content of this Game Wager order is different, please change the Game Wager.{info}",
				DateTime.Now.ToString("o"),
				new {
					model.Id,
					command.GameWagerPrimaryId,
					command.ExternalCreatedOn
				});

			await repository.UpdateAsync(
				source: new GameWagerSettlementUpdateModel(
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
					ExternalResettleOn: command.ExternalResettleOn),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
