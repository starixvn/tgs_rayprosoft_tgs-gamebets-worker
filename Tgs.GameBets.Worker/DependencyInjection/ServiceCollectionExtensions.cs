namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameBetsWorker(
		this IServiceCollection services)
		=> services
		.AddGameBetsWorkerCore()
		.AddGameBetsWorkerRepositories();
}
