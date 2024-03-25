using System.Text.Json.Nodes;
using Tgs.GameBets.Queue.Common.Enums;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record RoundCommand(
	Guid GameProviderId,
	Guid GameProviderConfigurationId,
	int GameId,
	Guid BrandId,
	string PlayerCode,
	string CurrencyCode,
	Guid WalletId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	BetState BetState,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	JsonNode AdditionalData);
