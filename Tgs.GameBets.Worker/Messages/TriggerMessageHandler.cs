using Tgs.GameBets.Worker.Common.Protos;

using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.ApplicationServices;

using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Messages;

internal class TriggerMessageHandler(
	ILogger<TriggerMessageHandler> logger,
	IGameBetsWorkerDispatcher dispatcher) : IMessageHandler<GameBetsWorkerMessage>
{
	public ValueTask HandleAsync(string subject, GameBetsWorkerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return dispatcher.PushCommandAsync(
			command: new GameBetWorkerCommand(
				GameProviderCode: data.GameProviderCode),
			cancellationToken: cancellationToken);
	}
}
