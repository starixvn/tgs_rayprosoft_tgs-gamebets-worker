using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameBets.Worker.Entities;

namespace Tgs.GameBets.Worker.Repositories;

internal class GameProviderRepository(
	TgsContext context) : IGameProviderRepository
{
	public async ValueTask<Guid> GetAsync(string code, CancellationToken cancellationToken = default)
		=> await context.GameProviders
		.Where(predicate: gp => gp.Code == code)
		.Select(selector: gp => gp.Id)
		.SingleAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public IAsyncEnumerable<GameProviderInfoModel> QueryAsync(CancellationToken cancellationToken = default)
		=> context.GameProviders
		.Where(predicate: gp => gp.Status != 1)
		.Select(selector: gp => new GameProviderInfoModel(
			gp.Id,
			gp.Code))
		.AsAsyncEnumerable();
}
