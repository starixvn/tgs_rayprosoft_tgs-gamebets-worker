using Tgs.ServiceUtility;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.JobApplicationServices;

namespace Tgs.GameBets.Worker.Core;

internal class HangfireUtensil(
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
