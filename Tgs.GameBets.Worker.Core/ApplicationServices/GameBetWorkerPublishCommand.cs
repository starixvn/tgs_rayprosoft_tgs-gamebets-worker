namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record GameBetWorkerPublishCommand(
	Guid GameProviderId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	GameBets.Common.Enums.GameBetCommandResult Result);
