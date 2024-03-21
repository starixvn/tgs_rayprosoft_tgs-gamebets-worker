using Microsoft.AspNetCore.Mvc;
using Valhalla.MessageQueue;
using Tgs.GameBets.Worker.Common;
using Tgs.GameBets.Worker.Common.Protos;

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
			subject: GameBetsWorkerNatsConst.Trigger,
			data: new GameBetsWorkerMessage
			{
				GameProviderCode = gameProviderCode,
			},
			cancellationToken: HttpContext.RequestAborted);
}
