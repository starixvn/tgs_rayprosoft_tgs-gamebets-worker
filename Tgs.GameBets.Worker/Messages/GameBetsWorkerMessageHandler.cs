using System.Text.Json;
using System.Text.Json.Nodes;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Messages;

internal class GameBetsWorkerMessageHandler(
	ILogger<GameBetsWorkerMessageHandler> logger,
	IGameBetsWorkerDispatcher dispatcher) : IMessageHandler<Common.Protos.GameBetsWorkerRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameBetsWorkerRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					GameProviderId = data.GameProviderId.ToGuid(),
					data.GameId,
					data.PlayerCode,
					WalletId = data.WalletId.ToGuid(),
					data.ExternalRoundId,
					data.ExternalBetId,
					ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new GameBetWorkerResponse(
				GameBetId: data.GameBetId.ToGuid(),
				GameProviderId: data.GameProviderId.ToGuid(),
				GameProviderConfigurationId: data.GameProviderConfigurationId.ToGuid(),
				GameId: data.GameId,
				ResellerId: data.ResellerId.ToGuid(),
				LicenseeId: data.LicenseeId.ToGuid(),
				BrandId: data.BrandId.ToGuid(),
				PlayerCode: data.PlayerCode,
				CurrencyCode: data.CurrencyCode,
				WalletId: data.WalletId.ToGuid(),
				ExternalRoundId: data.ExternalRoundId,
				ExternalBetId: data.ExternalBetId,
				ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn: data.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn: data.ExternalPayoffOn.ToDateTimeOffset(),
				ExternalResettleOn: data.ExternalResettleOn.ToDateTimeOffset(),
				BetState: (GameBets.Common.Enums.BetState)data.BetState,
				BetType: (GameBets.Common.Enums.BetType)data.BetType,
				FreeGameState: (GameBets.Common.Enums.FreeGameState)data.FreeGameState,
				GambleState: (GameBets.Common.Enums.GambleState)data.GambleState,
				CashoutState: (GameBets.Common.Enums.CashoutState)data.CashoutState,
				ResettleState: (GameBets.Common.Enums.ResettleState)data.ResettleState,
				JackpotState: (GameBets.Common.Enums.JackpotState)data.JackpotState,
				CurrencyRatio: data.CurrencyRatio.ToDecimal().GetValueOrDefault(0.0m),
				Bet: data.Bet.ToDecimal().GetValueOrDefault(0.0m),
				ValidBet: data.ValidBet.ToDecimal().GetValueOrDefault(0.0m),
				Settle: data.Settle.ToDecimal().GetValueOrDefault(0.0m),
				Commission: data.Commission.ToDecimal().GetValueOrDefault(0.0m),
				Turnover: data.Turnover.ToDecimal().GetValueOrDefault(0.0m),
				WinLoss: data.WinLoss.ToDecimal().GetValueOrDefault(0.0m),
				Jackpot: data.Jackpot.ToDecimal().GetValueOrDefault(0.0m),
				AdditionalData: JsonNode.Parse(data.AdditionalData)!),
			cancellationToken: cancellationToken);
	}
}
