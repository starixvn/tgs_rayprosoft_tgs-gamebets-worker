using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Repositories;
internal class GameBettingRepository : IGameBettingRepository
{
	public ValueTask AddAsync(GameBettingAddModel source, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public ValueTask<GameBettingInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public ValueTask UpdateAsync(GameBettingUpdateModel source, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
