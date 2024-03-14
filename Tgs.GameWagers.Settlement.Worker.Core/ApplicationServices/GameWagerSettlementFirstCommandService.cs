using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
internal class GameWagerSettlementFirstCommandService(
	IGameWagerSettlementFirstRepository gameWagerSettlementFirstRepository,
	IGameWagerSettlementRepository gameWagerSettlementRepository,
	ISerialNumberGenerator serialNumberGenerator) : ICommandService<GameWagerSettlementFirstCommand>
{
	public async ValueTask ExecuteAsync(GameWagerSettlementFirstCommand command, CancellationToken cancellationToken = default)
	{
		if (await gameWagerSettlementFirstRepository.ExistAsync(
			gameWagerPrimaryId: command.GameWagerPrimaryId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			return;
		}

		var model = await gameWagerSettlementRepository.GetAsync(
			gameWagerPrimaryId: command.GameWagerPrimaryId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(command.GameWagerPrimaryId.ToString());

		var id = await serialNumberGenerator.GenerateAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameWagerSettlementFirstRepository.AddAsync(
			source: new GameWagerSettlementFirstAddModel(
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
				UpdatedOn: model.UpdatedOn),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
