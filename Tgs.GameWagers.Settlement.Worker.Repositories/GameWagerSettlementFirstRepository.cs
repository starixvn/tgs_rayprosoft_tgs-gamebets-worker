using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.GameWagers.Settlement.Worker.Entities;

namespace Tgs.GameWagers.Settlement.Worker.Repositories;
internal class GameWagerSettlementFirstRepository(
	TgsContext context) : IGameWagerSettlementFirstRepository
{
	public async ValueTask AddAsync(GameWagerSettlementFirstAddModel source, CancellationToken cancellationToken = default)
	{
		var entity = new GameWagerSettlementFirst
		{
			Id = source.Id,
			FirstOn = DateTime.UtcNow,
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
			CreatedOn = source.CreatedOn.UtcDateTime,
			ClosedOn = source.ClosedOn.UtcDateTime,
			UpdatedOn = source.UpdatedOn.UtcDateTime
		};

		_ = await context.GameWagerSettlementFirsts.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameWagerSettlementFirsts
		.AnyAsync(gws => gws.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) && gws.GameWagerPrimaryId == gameWagerPrimaryId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
