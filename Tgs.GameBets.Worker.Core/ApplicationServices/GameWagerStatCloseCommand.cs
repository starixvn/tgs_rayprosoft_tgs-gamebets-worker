namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record GameWagerStatCloseCommand(
	Guid GameProviderId,
	int GameId,
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	DateTimeOffset ClosedOn);
