using System.Text.Json;
using System.Text.Json.Nodes;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Messages;

internal class GameBetRunningWorkerMessageHandler(
	ILogger<GameBetRunningWorkerMessageHandler> logger,
	IGameBetRunningWorkerDispatcher dispatcher) : IMessageHandler<Common.Protos.GameBetRunningWorkerRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameBetRunningWorkerRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
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
			request: new GameBetRunningWorkerRequest(
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
				BetState: (GameBets.Common.Enums.BetState)data.BetState,
				BetType: (GameBets.Common.Enums.BetType)data.BetType,
				CurrencyRatio: data.CurrencyRatio.ToDecimal().GetValueOrDefault(0.0m),
				Bet: data.Bet.ToDecimal().GetValueOrDefault(0.0m),
				ValidBet: data.ValidBet.ToDecimal().GetValueOrDefault(0.0m),
				CreatedOn: data.CreatedOn.ToDateTimeOffset(),
				AdditionalData: JsonNode.Parse(data.AdditionalData) ?? new JsonObject { },
				ExtendedData: JsonNode.Parse(data.ExtendedData) ?? new JsonObject { }),
			cancellationToken: cancellationToken);
	}
}
