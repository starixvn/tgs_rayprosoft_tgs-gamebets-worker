using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Common.Enums;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.GameWagers.Settlement.Worker.Entities;

namespace Tgs.GameWagers.Settlement.Worker.Repositories;
internal class GameWagerSettlementRepository(
	TgsContext context) : IGameWagerSettlementRepository
{
	public async ValueTask AddAsync(GameWagerSettlementAddModel source, CancellationToken cancellationToken = default)
	{
		var entity = new GameWagerSettlement
		{
			Id = source.Id,
			CreatedOn = DateTime.UtcNow,
			GameWagerPrimaryId = source.GameWagerPrimaryId,
			WagerState = (int)source.WagerState,
			WagerType = (int)source.WagerType,
			FreeGameState = (int)source.FreeGameState,
			GambleState = (int)source.GambleState,
			CashoutState = (int)source.CashoutState,
			ResettleState = (int)source.ResettleState,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			ExternalCreatedOn = source.ExternalCreatedOn.UtcDateTime,
			ExternalClosedOn = source.ExternalClosedOn.UtcDateTime,
			ExternalPayoffOn = source.ExternalPayoffOn.UtcDateTime,
			ExternalResettleOn = source.ExternalResettleOn.UtcDateTime,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			ResetState = (int)ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime
		};

		_ = await context.GameWagerSettlements.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameWagerSettlements
		.AnyAsync(gws => gws.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) && gws.GameWagerPrimaryId == gameWagerPrimaryId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<GameWagerSettlementInfoModel?> GetAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameWagerSettlements
		.Where(gws => gws.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) && gws.GameWagerPrimaryId == gameWagerPrimaryId)
		.Select(gws => new GameWagerSettlementInfoModel(
			gws.Id,
			gws.GameWagerPrimaryId,
			(WagerState)gws.WagerState,
			(WagerType)gws.WagerType,
			(FreeGameState)gws.FreeGameState,
			(GambleState)gws.GambleState,
			(CashoutState)gws.CashoutState,
			(ResettleState)gws.ResettleState,
			gws.CurrencyRatio,
			gws.Bet,
			gws.ValidBet,
			gws.Settle,
			gws.Commission,
			gws.Turnover,
			gws.WinLoss,
			gws.ExternalCreatedOn,
			gws.ExternalClosedOn,
			gws.ExternalPayoffOn,
			gws.ExternalResettleOn,
			gws.CreatedOn,
			gws.ClosedOn,
			gws.UpdatedOn
			))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask UpdateAsync(GameWagerSettlementUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameWagerSettlements
			.Where(gws => gws.ExternalCreatedOn >= source.ExternalCreatedOn.AddDays(-7) &&
			gws.Id == source.Id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		entity.WagerState = (int)source.WagerState;
		entity.WagerType = (int)source.WagerType;
		entity.FreeGameState = (int)source.FreeGameState;
		entity.GambleState = (int)source.GambleState;
		entity.CashoutState = (int)source.CashoutState;
		entity.ResettleState = (int)source.ResettleState;
		entity.Bet = source.Bet;
		entity.ValidBet = source.ValidBet;
		entity.Settle = source.Settle;
		entity.Commission = source.Commission;
		entity.Turnover = source.Turnover;
		entity.WinLoss = source.WinLoss;
		entity.ExternalResettleOn = source.ExternalResettleOn.UtcDateTime;
		entity.ResetState = (int)ResetState.Reset;
		entity.ResetOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow;

		_ = context.GameWagerSettlements.Update(entity: entity);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
