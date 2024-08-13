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
			command: new GameBetWorkerCommand(
				data.GameBetId.ToGuid(),
				data.GameProviderId.ToGuid(),
				data.GameProviderConfigurationId.ToGuid(),
				data.GameId,
				data.ResellerId.ToGuid(),
				data.LicenseeId.ToGuid(),
				data.BrandId.ToGuid(),
				data.PlayerCode,
				data.CurrencyCode,
				data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				data.ExternalCreatedOn.ToDateTimeOffset(),
				data.ExternalClosedOn.ToDateTimeOffset(),
				data.ExternalPayoffOn.ToDateTimeOffset(),
				data.ExternalResettleOn.ToDateTimeOffset(),
				(GameBets.Common.Enums.BetState)data.BetState,
				(GameBets.Common.Enums.BetType)data.BetType,
				(GameBets.Common.Enums.FreeGameState)data.FreeGameState,
				(GameBets.Common.Enums.GambleState)data.GambleState,
				(GameBets.Common.Enums.CashoutState)data.CashoutState,
				(GameBets.Common.Enums.ResettleState)data.ResettleState,
				(GameBets.Common.Enums.JackpotState)data.JackpotState,
				data.CurrencyRatio.ToDecimal().GetValueOrDefault(0.0m),
				data.Bet.ToDecimal().GetValueOrDefault(0.0m),
				data.ValidBet.ToDecimal().GetValueOrDefault(0.0m),
				data.Settle.ToDecimal().GetValueOrDefault(0.0m),
				data.Commission.ToDecimal().GetValueOrDefault(0.0m),
				data.Turnover.ToDecimal().GetValueOrDefault(0.0m),
				data.WinLoss.ToDecimal().GetValueOrDefault(0.0m),
				data.Jackpot.ToDecimal().GetValueOrDefault(0.0m),
				JsonNode.Parse(data.AdditionalData)!),
			cancellationToken: cancellationToken);
	}
}
