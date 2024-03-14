using Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;

namespace Tgs.GameWagers.Settlement.Worker.Core;

public interface IGameWagersSettlementWorkerDispatcher
{
    ValueTask PushCommandAsync(GameWagerSettlementWorkerCommand command, CancellationToken cancellationToken = default);
}
