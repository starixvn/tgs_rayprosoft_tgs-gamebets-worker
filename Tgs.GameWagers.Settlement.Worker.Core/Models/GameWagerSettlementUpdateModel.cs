using Tgs.GameWagers.Common.Enums;

namespace Tgs.GameWagers.Settlement.Worker.Core.Models;
public record GameWagerSettlementUpdateModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	WagerState WagerState,
	WagerType WagerType,
	FreeGameState FreeGameState,
	GambleState GambleState,
	CashoutState CashoutState,
	ResettleState ResettleState,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	DateTimeOffset ExternalResettleOn);
