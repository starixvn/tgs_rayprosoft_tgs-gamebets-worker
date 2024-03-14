using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Core;

public interface IGameProviderRepository
{
	IAsyncEnumerable<GameProviderInfoModel> QueryAsync(CancellationToken cancellationToken = default);

	ValueTask<Guid> GetAsync(string code, CancellationToken cancellationToken = default);
}
