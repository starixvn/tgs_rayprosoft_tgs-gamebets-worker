using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Core;
public interface IGameWagerSettlementFirstRepository
{
	ValueTask AddAsync(GameWagerSettlementFirstAddModel source, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);
}
