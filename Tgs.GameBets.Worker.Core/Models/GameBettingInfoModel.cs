using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Tgs.GameBets.Worker.Core.Models;
public record GameBettingInfoModel(
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