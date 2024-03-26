using Microsoft.AspNetCore.Mvc;
using Tgs.GameBets.Worker.Common;
using Tgs.GameBets.Worker.Common.Protos;

using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Controllers;

[Route("game-bets-Worker")]
[ApiController]
public class GameBetsWorkerController : ControllerBase
{
	[HttpPost("{gameProviderCode}/trigger")]
	public ValueTask TriggerAsync(
		[FromServices] IMessageSender messageSender,
		string gameProviderCode)
		=> messageSender.PublishAsync(
			subject: GameBetsWorkerNatsConst.GameBetsWorkerTrigger,
			data: new GameBetsWorkerMessage
			{
				GameProviderCode = gameProviderCode,
			},
			cancellationToken: HttpContext.RequestAborted);
}
