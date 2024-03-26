using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Entities;
using Tgs.GameBets.Worker.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsWorkerRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IGameProviderRepository, GameProviderRepository>()
		.AddTransient<IRoundRepository, RoundRepository>()
		.Decorate<IRoundRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<RoundLockedCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.AddTransient<IGameBetRepository, GameBetRepository>()
		.Decorate<IGameBetRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameBetLockedCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)));
}
