using System.Text.Json.Nodes;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBetUpdateModel(
	Guid Id,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalResettleOn,
	Common.Protos.BetState BetState,
	Common.Protos.BetType BetType,
	Common.Protos.FreeGameState FreeGameState,
	Common.Protos.GambleState GambleState,
	Common.Protos.CashoutState CashoutState,
	Common.Protos.ResettleState ResettleState,
	Common.Protos.JackpotState JackpotState,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	decimal Jackpot,
	JsonNode AdditionalData);
