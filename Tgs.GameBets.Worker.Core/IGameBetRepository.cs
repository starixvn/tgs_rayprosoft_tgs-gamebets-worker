using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetRepository
{
	ValueTask CommandAsync(GameBetCommandModel source, CancellationToken cancellationToken = default);

	ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
