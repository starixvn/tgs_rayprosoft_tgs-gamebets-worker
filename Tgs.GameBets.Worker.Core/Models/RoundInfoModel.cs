namespace Tgs.GameBets.Worker.Core.Models;
public record RoundInfoModel(
	Guid Id,
	Guid GameProviderId,
	string ExternalRoundId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	RoundStatus Status,
	RoundClientStatus ClientStatus,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	DateTimeOffset? ReopenedOn);