using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetsWorkerDispatcher
{
	ValueTask PushCommandAsync(GameBetWorkerRequest request, CancellationToken cancellationToken = default);

	void ExtendExpiry(Guid brandId, Guid gameProviderId, int gameId, CancellationToken cancellationToken = default);
}
