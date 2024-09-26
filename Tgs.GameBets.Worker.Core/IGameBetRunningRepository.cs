using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetRunningRepository
{
	ValueTask CommandAsync(GameBetRunningCommand source, CancellationToken cancellationToken = default);

	ValueTask DeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default);
}
