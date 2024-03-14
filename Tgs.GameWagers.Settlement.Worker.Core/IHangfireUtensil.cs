using Tgs.GameWagers.Settlement.Worker.Core.JobApplicationServices;

namespace Tgs.GameWagers.Settlement.Worker.Core;

public interface IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
