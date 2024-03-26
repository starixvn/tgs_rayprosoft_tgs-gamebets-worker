using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameBets.Worker.Entities;

namespace Tgs.GameBets.Worker.Repositories;

internal class GameBetFirstRepository(
	TgsContext context) : IGameBetFirstRepository
{
	public async ValueTask AddAsync(GameBetFirstAddModel source, CancellationToken cancellationToken = default)
	{
		context.ChangeTracker.Clear();

		var entity = new GameBetFirst
		{
			Id = source.Id,
			GameBetId = source.GameBetId,
			GameProviderId = source.GameProviderId,
			GameProviderConfigurationId = source.GameProviderConfigurationId,
			GameId = source.GameId,
			ResellerId = source.ResellerId,
			LicenseeId = source.LicenseeId,
			BrandId = source.BrandId,
			PlayerCode = source.PlayerCode,
			CurrencyCode = source.CurrencyCode,
			WalletId = source.WalletId,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalCreatedOn = source.ExternalCreatedOn.UtcDateTime,
			ExternalClosedOn = source.ExternalClosedOn.UtcDateTime,
			ExternalPayoffOn = source.ExternalPayoffOn.UtcDateTime,
			ExternalResettleOn = source.ExternalResettleOn.UtcDateTime,
			BetState = (int)source.BetState,
			BetType = (int)source.BetType,
			FreeGameState = (int)source.FreeGameState,
			GambleState = (int)source.GambleState,
			CashoutState = (int)source.CashoutState,
			ResettleState = (int)source.ResettleState,
			JackpotState = (int)source.JackpotState,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			Jackpot = source.Jackpot,
			CreatedOn = source.CreatedOn.UtcDateTime,
			ClosedOn = source.ClosedOn.UtcDateTime,
			UpdatedOn = source.UpdatedOn.UtcDateTime,
			FirstOn = DateTime.UtcNow,
			AdditionalData = source.AdditionalData.ToJsonString()
		};

		_ = await context.GameBetFirsts.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameBetFirsts
		.AnyAsync(gbs => gbs.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbs.GameProviderId == gameProviderId &&
			gbs.GameId == gameId &&
			gbs.WalletId == walletId &&
			gbs.ExternalBetId == externalBetId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
