using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetsWorkerDispatcher
{
	ValueTask PushCommandAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default);
}
