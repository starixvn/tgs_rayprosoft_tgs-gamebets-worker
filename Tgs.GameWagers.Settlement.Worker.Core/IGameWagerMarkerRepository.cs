using System.Text.Json.Nodes;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Core;
public interface IGameWagerMarkerRepository
{
	public ValueTask AddAsync(string code, string directions, CancellationToken cancellationToken = default);
	public ValueTask UpdateAsync(string code, Guid marker, CancellationToken cancellationToken = default);
	public ValueTask<GameWagerMarkerInfoModel?> GetAsync(string code, CancellationToken cancellationToken = default);
}
