using Tgs.GameWagers.Settlement.Worker.Common.Protos;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Settlement.Worker.Messages;

internal class TriggerMessageHandler(
	ILogger<TriggerMessageHandler> logger,
	IGameWagersSettlementWorkerDispatcher dispatcher) : IMessageHandler<GameWagersSettlementWorkerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersSettlementWorkerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return dispatcher.PushCommandAsync(
			command: new GameWagerSettlementWorkerCommand(
				GameProviderCode: data.GameProviderCode),
			cancellationToken: cancellationToken);
	}
}