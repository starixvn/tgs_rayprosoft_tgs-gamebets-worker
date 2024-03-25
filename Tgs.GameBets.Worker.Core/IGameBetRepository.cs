using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetRepository
{
	ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default);
}
