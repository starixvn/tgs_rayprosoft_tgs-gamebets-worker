using System.Text.Json.Nodes;
using MediatR;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
public record GameBetRunningWorkerRequest(
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
	GameBets.Common.Enums.BetState BetState,
	GameBets.Common.Enums.BetType BetType,
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	DateTimeOffset CreatedOn,
	JsonNode AdditionalData,
	JsonNode ExtendedData) : IRequest;
