using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelCommand.Client;
using Tgs.GameBets.Worker;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameBetsWorker(
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
		.AddGameBetsWorkerCore()
		.AddGameBetsWorkerRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
