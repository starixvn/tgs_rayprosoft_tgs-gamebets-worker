namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record GameBetFirstCommand(
	Guid GameProviderId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	Queue.Common.Enums.BetState BetState);
