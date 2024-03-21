using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Tgs.GameBets.Worker.Common;
using Tgs.GameBets.Worker.Common.Protos;

namespace Tgs.GameBets.Worker.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{

		var message = new GameBetsWorkerMessage
		{
			GameProviderCode = command.GameProviderCode,
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This publish message.({message})",
			DateTime.Now.ToString("u"),
			GameBetsWorkerNatsConst.Trigger,
			message);

		return messageSender.PublishAsync(
			subject: GameBetsWorkerNatsConst.Trigger,
			data: message,
			cancellationToken: cancellationToken);
	}
}
