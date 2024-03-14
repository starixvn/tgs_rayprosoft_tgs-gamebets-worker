using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Settlement.Worker.Common;
using Tgs.GameWagers.Settlement.Worker.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Settlement.Worker.Controllers;
[Route("Settlement-Worker")]
[ApiController]
public class SettlementWorkerController : ControllerBase
{
	[HttpPost("{gameProviderCode}/trigger")]
	public ValueTask TriggerAsync(
		[FromServices] IMessageSender messageSender,
		string gameProviderCode)
		=> messageSender.PublishAsync(
			subject: GameWagersSettlementWorkerNatsConst.Trigger,
			data: new GameWagersSettlementWorkerMessage
			{
				GameProviderCode = gameProviderCode,
			},
			cancellationToken: HttpContext.RequestAborted);
}
