using System.Reflection;
using Tgs.GameWagers.Settlement.Worker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameWagersSettlementWorkerCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSerialNumberGenerator()
		.AddTransient<IHangfireUtensil, HangfireUtensil>()
		.AddSingleton<IGameWagersSettlementWorkerDispatcher, GameWagersSettlementWorkerDispatcher>();
}
