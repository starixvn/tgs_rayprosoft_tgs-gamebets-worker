using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IGameBetFirstRepository
{
	ValueTask AddAsync(GameBetFirstAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
