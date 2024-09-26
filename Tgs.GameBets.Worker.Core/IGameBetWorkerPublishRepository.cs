using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetWorkerPublishRepository
{
	ValueTask PublishAsync(GameBetWorkerPublish source, CancellationToken cancellationToken = default);
}
