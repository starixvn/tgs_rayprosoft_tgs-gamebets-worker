namespace Tgs.GameBets.Worker.Core.Models;
public record RoundClosedModel(
	RoundClientStatus ClientStatus,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss);
