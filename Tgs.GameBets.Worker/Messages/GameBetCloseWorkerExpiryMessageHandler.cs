using System.Text.Json;
using Tgs.GameBets.Worker.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Messages;

internal class GameBetCloseWorkerExpiryMessageHandler(
	ILogger<GameBetCloseWorkerExpiryMessageHandler> logger,
	IGameBetCloseWorkerDispatcher dispatcher) : IMessageHandler<Common.Protos.GameBetCloseWorkerExpiryRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameBetCloseWorkerExpiryRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					BrandId = data.BrandId.ToGuid(),
					GameProviderId = data.GameProviderId.ToGuid(),
					data.GameId
				},
				Message = "This receive message handler."
			}));

		dispatcher.ExtendExpiry(
			data.BrandId.ToGuid(),
			data.GameProviderId.ToGuid(),
			data.GameId,
			cancellationToken);

		return ValueTask.CompletedTask;
	}
}
