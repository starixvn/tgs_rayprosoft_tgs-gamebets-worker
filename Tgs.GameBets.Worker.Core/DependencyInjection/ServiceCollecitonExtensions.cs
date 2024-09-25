using System.Reflection;
using Tgs.GameBets.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsWorkerCore(
		this IServiceCollection services)
		=> services
		.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
		.AddSingleton<IGameBetsWorkerDispatcher, GameBetsWorkerDispatcher>();
}
