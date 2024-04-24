using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core;

internal class GameBetsWorkerChannel(
	ILogger<GameBetsWorkerChannel> logger,
	ICommandService<GameBetWorkerCommand> service,
	string key) : IDisposable, IAsyncDisposable
{
	private static readonly ActivitySource _ActivitySource = new(name: typeof(GameBetsWorkerChannel).FullName!);

	private readonly Channel<GameBetWorkerCommand> _channel = Channel.CreateUnbounded<GameBetWorkerCommand>(
		new UnboundedChannelOptions
		{
			SingleReader = true
		});

	private readonly SemaphoreSlim _locker = new(1, 1);
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private Task? _backgroundTask;
	private bool _disposedValue;

	public event EventHandler? Disposed;

	public string Key { get; } = key;

	~GameBetsWorkerChannel()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: false);
	}

	public async ValueTask PrepareAsync(CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

		if (_backgroundTask is not null)
			return;

		await _locker.WaitAsync(cts.Token).ConfigureAwait(false);

		try
		{
			if (_backgroundTask is not null)
				return;

			_backgroundTask = Task.Run(
				() => ProcessCommandLoopAsync(_cancellationTokenSource.Token),
				_cancellationTokenSource.Token);
		}
		finally
		{
			_ = _locker.Release();
		}
	}

	public async ValueTask PushCommandAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(
			cancellationToken,
			_cancellationTokenSource.Token);

		await _channel.Writer.WriteAsync(
			command,
			cts.Token).ConfigureAwait(false);
	}

	public ValueTask DisposeAsync()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

		return ValueTask.CompletedTask;
	}

	public void Dispose()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				Disposed?.Invoke(this, EventArgs.Empty);
				_channel.Writer.Complete();
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}

	private async Task ProcessCommandLoopAsync(CancellationToken cancellationToken = default)
	{
		await Task.Yield();

		var reader = _channel.Reader;
		while (await reader
			.WaitToReadAsync(cancellationToken)
			.ConfigureAwait(false))
		{
			if (reader.TryRead(out var command))
			{
				using var activity = _ActivitySource.StartActivity(
					"GameBetsWorkerChannel handle exchange",
					 ActivityKind.Internal,
					parentContext: default);
				try
				{
					logger.LogInformation("LogOn:{logOn} Key:{key} Mesaage:This processing.({data})",
						DateTime.Now.ToString("o"),
						Key,
						new
						{
							command.GameProviderId,
							command.GameId,
							command.PlayerCode,
							command.WalletId,
							command.ExternalRoundId,
							command.ExternalBetId,
							command.ExternalCreatedOn
						});

					await service.ExecuteAsync(
						command: command,
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Key:{key} Mesaage:This exception occurs.({data})",
						DateTime.Now.ToString("o"),
						Key,
						new
						{
							command.GameProviderId,
							command.GameId,
							command.PlayerCode,
							command.WalletId,
							command.ExternalRoundId,
							command.ExternalBetId,
							command.ExternalCreatedOn
						});
				}
			}
		}
	}
}
