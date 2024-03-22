using System.Text.Json.Nodes;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBettingAddModel(
	Guid Id,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalBetId,
	Guid GameProviderConfigurationId,
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	string CurrencyCode,
	string ExternalRoundId,
	int BetType,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	JsonNode AdditionalData);