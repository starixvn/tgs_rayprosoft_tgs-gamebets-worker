namespace Tgs.GameBets.Worker.Core.Models;
public record GameBetWorkerPublish(
	Guid GameProviderId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	GameBets.Common.Enums.GameBetCommandResult Result);
