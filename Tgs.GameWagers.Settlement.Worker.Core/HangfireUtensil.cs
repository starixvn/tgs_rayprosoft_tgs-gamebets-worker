using Tgs.ServiceUtility;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.JobApplicationServices;

namespace Tgs.GameWagers.Settlement.Worker.Core;

internal class HangfireUtensil(
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
