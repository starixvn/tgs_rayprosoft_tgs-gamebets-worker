using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Settlement.Worker;
using Tgs.VipLevelCommand.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersSettlementWorker(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions,
		Action<GameWagersClientOptions, IServiceProvider> gameWagersClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameWagersClient(gameWagersClientOptions)
		.AddGameWagersSettlementWorkerCore()
		.AddGameWagersSettlementWorkerRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
