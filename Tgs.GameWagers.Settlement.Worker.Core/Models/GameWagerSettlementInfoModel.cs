using System.Reflection;
using Tgs.GameWagers.Common.Enums;

namespace Tgs.GameWagers.Settlement.Worker.Core.Models;
public record GameWagerSettlementInfoModel(
	Guid Id,
	Guid GameWagerPrimaryId,
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
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	DateTimeOffset ExternalPayoffOn,
	DateTimeOffset ExternalResettleOn,
	DateTimeOffset CreatedOn,
	DateTimeOffset ClosedOn,
	DateTimeOffset UpdatedOn);