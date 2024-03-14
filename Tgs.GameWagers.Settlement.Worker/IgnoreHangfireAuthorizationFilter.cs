using Hangfire.Dashboard;

namespace Tgs.GameWagers.Settlement.Worker;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
