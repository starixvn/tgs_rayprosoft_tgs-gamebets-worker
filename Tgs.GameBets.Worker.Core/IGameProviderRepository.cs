using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;

public interface IGameProviderRepository
{
	IAsyncEnumerable<GameProviderInfoModel> QueryAsync(CancellationToken cancellationToken = default);

	ValueTask<Guid> GetAsync(string code, CancellationToken cancellationToken = default);
}
