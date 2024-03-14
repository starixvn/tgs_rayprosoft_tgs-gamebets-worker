using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Settlement.Worker.Common.Protos;
using Tgs.GameWagers.Settlement.Worker.Common;

namespace Tgs.GameWagers.Settlement.Worker.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{

		var message = new GameWagersSettlementWorkerMessage
		{
			GameProviderCode = command.GameProviderCode,
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This publish message.({message})",
			DateTime.Now.ToString("u"),
			GameWagersSettlementWorkerNatsConst.Trigger,
			message);

		return messageSender.PublishAsync(
			subject: GameWagersSettlementWorkerNatsConst.Trigger,
			data: message,
			cancellationToken: cancellationToken);
	}
}
