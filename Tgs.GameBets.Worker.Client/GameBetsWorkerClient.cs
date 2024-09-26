using System.Net.Http.Json;
using System.Text.Json;

using System.Text.Json.Serialization;
using Tgs.GameBets.Worker.Common.ViewModels;

namespace Tgs.GameBets.Worker.Client;

internal class GameBetsWorkerClient(
	HttpClient httpClient) : IGameBetsWorkerClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public async ValueTask CloseHandleAsync(GameBetCloseHandleViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var httpResponseMessage = await httpClient.PostAsync(
			"/handles/close",
			requestContent,
			cancellationToken)
			.ConfigureAwait(false);

		_ = httpResponseMessage.EnsureSuccessStatusCode();
	}

	public async ValueTask HandleAsync(WorkerHandleViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var httpResponseMessage = await httpClient.PostAsync(
			"/worker",
			requestContent,
			cancellationToken)
			.ConfigureAwait(false);

		_ = httpResponseMessage.EnsureSuccessStatusCode();
	}

	public async ValueTask RunningHandleAsync(GameBetRunningHandleViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var httpResponseMessage = await httpClient.PostAsync(
			"/handles/running",
			requestContent,
			cancellationToken)
			.ConfigureAwait(false);

		_ = httpResponseMessage.EnsureSuccessStatusCode();
	}
}
