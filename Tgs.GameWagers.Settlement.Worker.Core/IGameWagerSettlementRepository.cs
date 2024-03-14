using Tgs.GameWagers.Common;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Core;
public interface IGameWagerSettlementRepository
{
	ValueTask AddAsync(GameWagerSettlementAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<GameWagerSettlementInfoModel?> GetAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(GameWagerSettlementUpdateModel source, CancellationToken cancellationToken = default);
}
