using System.Reflection;
using Tgs.GameBets.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameBetsWorkerCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSerialNumberGenerator()
		.AddTransient<IHangfireUtensil, HangfireUtensil>()
		.AddSingleton<IGameBetsWorkerDispatcher, GameBetsWorkerDispatcher>()
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>();
}
