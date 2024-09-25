using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IGameBetClosesRepository
{
	ValueTask<GameBets.Common.Enums.GameBetCommandResult> CommandAsync(GameBetCloseCommand source, CancellationToken cancellationToken = default);
}
