using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameBets.Worker.Core;

namespace Tgs.GameBets.Worker.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
	[HttpGet]
	public async Task<object> GetAsync(
		[FromServices] IWebHostEnvironment webHostEnvironment,
		[FromServices] IOptions<GameBetsWorkerSettingsOptions> gameBetsWorkerSettingsOptions,
		[FromServices] IOptions<PolicySettingsOptions> policySettingsOptions)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);

		return new
		{
			Environment.MachineName,
			HostName = hostName,
			HostIp = hostIp?.ToString() ?? string.Empty,
			Environment = webHostEnvironment.EnvironmentName,
			OsVersion = $"{Environment.OSVersion}",
			Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
			ProcessCount = Environment.ProcessorCount,
			GameBetsWorkerSettingsOptions = gameBetsWorkerSettingsOptions.Value,
			PolicySettingsOptions = policySettingsOptions.Value
		};
	}
}
