using System.Text.Json.Nodes;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBettingUpdateModel(
	Guid Id,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	JsonNode AdditionalData);