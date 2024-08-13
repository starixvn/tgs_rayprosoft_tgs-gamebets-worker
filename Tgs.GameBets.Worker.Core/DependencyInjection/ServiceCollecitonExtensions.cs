using System.Reflection;
using Tgs.GameBets.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsWorkerCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
		.AddSingleton<IGameBetsWorkerDispatcher, GameBetsWorkerDispatcher>();
}
