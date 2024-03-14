using System.Runtime.InteropServices;
using Hangfire;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Hangfire.Storage;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Core.JobApplicationServices;

internal class JobInitCommandService : ICommandService<JobInitCommand>
{
	private readonly ILogger<JobInitCommandService> _logger;
	private readonly IGameProviderRepository _repository;
	private readonly RecurringJobOptions _recurringJobOptions;

	public JobInitCommandService(
		ILogger<JobInitCommandService> logger,
		IGameProviderRepository repository)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_recurringJobOptions = new RecurringJobOptions()
		{
			TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
		};
	}

	public async ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
	{
		using var connection = JobStorage.Current.GetConnection();
		foreach (var recurringJob in connection.GetRecurringJobs())
		{
			RecurringJob.RemoveIfExists(recurringJob.Id);
		}

		try
		{
			var gameProviderInfos = await _repository.QueryAsync(
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameProviderInfoModel>();

			foreach (var gameProviderInfo in gameProviderInfos)
			{
				var jobExecuteCommand = new JobExecuteCommand(
					GameProviderCode: gameProviderInfo.Code);

				RecurringJob.AddOrUpdate<IHangfireUtensil>(
					recurringJobId: string.Concat(gameProviderInfo.Code, gameProviderInfo),
					methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
					cronExpression: Cron.Minutely(),
					options: _recurringJobOptions);

				//初始化強制第一次處理
				_ = BackgroundJob.Enqueue<IHangfireUtensil>(service => service.JobExecuteAsync(jobExecuteCommand));
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "LogOn:{logOn} Message:初始化排程發生例外", DateTime.Now.ToString("o"));
		}
	}
}
