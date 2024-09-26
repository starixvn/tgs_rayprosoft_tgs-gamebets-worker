using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetCloseWorkerDispatcher
{
	ValueTask PushCommandAsync(GameBetCloseWorkerRequest request, CancellationToken cancellationToken = default);

	void ExtendExpiry(Guid brandId, Guid gameProviderId, int gameId, CancellationToken cancellationToken = default);
}
