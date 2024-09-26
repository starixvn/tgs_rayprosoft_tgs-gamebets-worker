using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameBetRepository
{
	ValueTask<GameBets.Common.Enums.GameBetCommandResult> CommandAsync(GameBetCommandModel source, CancellationToken cancellationToken = default);
}
