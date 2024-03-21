using System.Text.Json.Nodes;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IGameBetMarkerRepository
{
	public ValueTask AddAsync(string code, string directions, CancellationToken cancellationToken = default);
	public ValueTask UpdateAsync(string code, Guid marker, CancellationToken cancellationToken = default);
	public ValueTask<GameBetMarkerInfoModel?> GetAsync(string code, CancellationToken cancellationToken = default);
}
