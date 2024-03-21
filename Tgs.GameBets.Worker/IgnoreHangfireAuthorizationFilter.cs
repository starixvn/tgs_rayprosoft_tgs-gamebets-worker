using Hangfire.Dashboard;

namespace Tgs.GameBets.Worker;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
