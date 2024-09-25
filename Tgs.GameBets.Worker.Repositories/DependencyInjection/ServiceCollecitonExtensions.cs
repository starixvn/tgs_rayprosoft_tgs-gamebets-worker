using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsWorkerRepositories(
		this IServiceCollection services)
		=> services
		.AddTransient<IGameBetRepository, GameBetRepository>()
		.AddTransient<IGameBetClosesRepository, GameBetClosesRepository>()
		.AddTransient<IGameBetRunningRepository, GameBetRunningRepository>()
		.AddTransient<IGameBetWorkerPublishRepository, GameBetWorkerPublishRepository>();
}
