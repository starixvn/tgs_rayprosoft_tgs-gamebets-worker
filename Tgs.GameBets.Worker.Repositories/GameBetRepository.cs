using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Common.Enums;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameBets.Worker.Entities;

namespace Tgs.GameBets.Worker.Repositories;
internal class GameBetRepository(
	TgsContext context) : IGameBetRepository
{
	public async ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default)
	{
		var entity = new GameBet
		{
			Id = source.Id,
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
			ExternalWagerId = source.ExternalWagerId,
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
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			ResetState = (int)ResetState.None,
			ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
			AdditionalData = source.AdditionalData.ToJsonString()
		};

		_ = await context.GameBets.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameBets
		.AnyAsync(gbs => gbs.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbs.GameProviderId == gameProviderId &&
			gbs.GameId == gameId &&
			gbs.WalletId == walletId &&
			gbs.ExternalWagerId == externalWagerId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> await context.GameBets
		.Where(gbs => gbs.ExternalCreatedOn >= externalCreatedOn.UtcDateTime.AddDays(-7) &&
			gbs.GameProviderId == gameProviderId &&
			gbs.GameId == gameId &&
			gbs.WalletId == walletId &&
			gbs.ExternalWagerId == externalWagerId)
		.Select(gws => new GameBetInfoModel(
			gws.Id,
			gws.GameProviderId,
			gws.GameProviderConfigurationId,
			gws.GameId,
			gws.ResellerId,
			gws.LicenseeId,
			gws.BrandId,
			gws.PlayerCode,
			gws.CurrencyCode,
			gws.WalletId,
			gws.ExternalRoundId,
			gws.ExternalWagerId,
			gws.ExternalCreatedOn,
			gws.ExternalClosedOn,
			gws.ExternalPayoffOn,
			gws.ExternalResettleOn,
			(BetState)gws.BetState,
			(BetType)gws.BetType,
			(FreeGameState)gws.FreeGameState,
			(GambleState)gws.GambleState,
			(CashoutState)gws.CashoutState,
			(ResettleState)gws.ResettleState,
			(JackpotState)gws.JackpotState,
			gws.CurrencyRatio,
			gws.Bet,
			gws.ValidBet,
			gws.Settle,
			gws.Commission,
			gws.Turnover,
			gws.WinLoss,
			gws.Jackpot,
			gws.CreatedOn,
			gws.ClosedOn,
			gws.UpdatedOn,
			gws.AdditionalData))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBets
			.Where(gws => gws.ExternalCreatedOn >= source.ExternalCreatedOn.AddDays(-7) &&
			gws.Id == source.Id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		entity.BetState = (int)source.BetState;
		entity.BetType = (int)source.BetType;
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
		entity.AdditionalData = source.AdditionalData.ToJsonString();

		_ = context.GameBets.Update(entity: entity);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
