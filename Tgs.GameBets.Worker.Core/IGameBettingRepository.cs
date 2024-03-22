using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IGameBettingRepository
{
	public ValueTask<GameBettingInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, CancellationToken cancellationToken = default);
	public ValueTask AddAsync(GameBettingAddModel source, CancellationToken cancellationToken = default);

	public ValueTask UpdateAsync(GameBettingUpdateModel source, CancellationToken cancellationToken = default);

	public ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
