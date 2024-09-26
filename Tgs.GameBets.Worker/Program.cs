using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Messages;

using Tgs.MessageQueue.Nats.Serializers;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
		c.IncludeXmlComments(xmlFileName);
});

builder.Services
	.AddOptions<PolicySettingsOptions>()
	.Configure((options) => builder.Configuration.GetSection("PolicySettings").Bind(options))
	.Services
	.AddOptions<GameBetsWorkerSettingsOptions>()
	.Configure((options) => builder.Configuration.GetSection("GameBetsWorkerSettings").Bind(options))
	.Services
	.AddGameBetsClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameBetsWorkerStrings:GameBetsClientBaseUrl");
	}))
	.AddGameBetsWorkerCore()
	.AddGameBetsWorkerRepositories();

builder.Services
	.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
	{
		Url = builder.Configuration.GetConnectionString("Nats")!,
		SerializerRegistry = TgsNatsSerializerRegistration.Default
	}))
	.AddMessageQueue()
	.AddNatsMessageQueue(config => config
	.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
	.AddHandler<GameBetWorkerMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerRequest, Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerRequest)
	.AddHandler<GameBetWorkerExpiryMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, string.Concat(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, ".", Environment.MachineName))
	.AddHandler<GameBetCloseWorkerMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetCloseWorkerRequest, Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetCloseWorkerRequest)
	.AddHandler<GameBetCloseWorkerExpiryMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, string.Concat(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, ".close.", Environment.MachineName))
	.AddHandler<GameBetRunningWorkerMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetRunningWorkerRequest, Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetRunningWorkerRequest)
	.AddHandler<GameBetRunningWorkerExpiryMessageHandler>(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, string.Concat(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest, ".running.", Environment.MachineName)))
	.ClearAllExchanges()
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerRequest)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetCloseWorkerRequest)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetRunningWorkerRequest)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.GameBetsWorkerNatsConst.GameBetWorkerExpiryRequest)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.Common.GameBetsWorkerNatsConst.GameBetsWorkerPublish)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.Common.GameBetsWorkerNatsConst.GameBetsWorkerAddPublish)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Worker.Common.GameBetsWorkerNatsConst.GameBetsWorkerUpdatePublish);

builder.Services
	.AddOpenTelemetry()
	.WithTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddSource("MediatR")
		.AddAspNetCoreInstrumentation(options => options.Filter =
		httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.MapControllers();
_ = app.MapSwagger();

app.UseHealthChecks("/healthz");

app.Run();
