using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace Tgs.GameBets.Worker.IntegrationTest;

internal class GameBetsWorkerApplication(Action<IWebHostBuilder>? webHostConfigure = null) : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		_ = builder
			.ConfigureAppConfiguration(c => c.Sources.Clear())
			.ConfigureServices(services => _ = services.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
				sp => new ConfigureNamedOptions<OtlpExporterOptions>(
					Options.DefaultName,
					exporterOptions => exporterOptions.TimeoutMilliseconds = 0))
			.AddFakeNatsMessageQueue());
		webHostConfigure?.Invoke(builder);
	}
}
