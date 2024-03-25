using System.Text.Json.Nodes;
using Tgs.GameBets.Queue.Common.Enums;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBetUpdateModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalResettleOn,
	BetState BetState,
	BetType BetType,
	FreeGameState FreeGameState,
	GambleState GambleState,
	CashoutState CashoutState,
	ResettleState ResettleState,
	JackpotState JackpotState,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	decimal Jackpot,
	JsonNode AdditionalData);
