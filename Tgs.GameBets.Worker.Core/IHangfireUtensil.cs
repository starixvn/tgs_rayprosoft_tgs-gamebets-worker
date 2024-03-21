using Tgs.GameBets.Worker.Core.JobApplicationServices;

namespace Tgs.GameBets.Worker.Core;

public interface IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
