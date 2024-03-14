using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Entities;
using Tgs.GameWagers.Settlement.Worker.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameWagersSettlementWorkerRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IGameProviderRepository, GameProviderRepository>()
		.AddTransient<IGameWagerMarkerRepository, GameWagerMarkerRepository>()
		.AddTransient<IGameWagerSettlementRepository, GameWagerSettlementRepository>()
		.Decorate<IGameWagerSettlementRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameWagerSettlementLockedCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.AddTransient<IGameWagerSettlementFirstRepository, GameWagerSettlementFirstRepository>()
		.Decorate<IGameWagerSettlementFirstRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameWagerSettlementFirstLockedCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)));

}
