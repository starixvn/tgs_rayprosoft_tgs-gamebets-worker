using System;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record GameWagerStatCloseCommand(
	Guid GameProviderId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn);
