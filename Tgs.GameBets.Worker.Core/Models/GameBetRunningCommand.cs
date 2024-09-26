using System.Text.Json.Nodes;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBetRunningCommand(
	Guid GameBetId,
	Guid GameProviderId,
	Guid GameProviderConfigurationId,
	int GameId,
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	string CurrencyCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	GameBets.Common.Enums.BetType BetType,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	DateTimeOffset CreatedOn,
	JsonNode AdditionalData,
	JsonNode ExtendedData);
