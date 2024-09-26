using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

internal class GameBetsWorkerChannel(
	ILogger<GameBetsWorkerChannel> logger,
	IOptions<GameBetsWorkerSettingsOptions> options,
	IServiceProvider serviceProvider,
	string key) : IDisposable, IAsyncDisposable
{
	private static readonly ActivitySource _ActivitySource = new(name: typeof(GameBetsWorkerChannel).FullName!);

	private readonly Channel<GameBetWorkerRequest> _channel = Channel.CreateUnbounded<GameBetWorkerRequest>(
		new UnboundedChannelOptions
		{
			SingleReader = true
		});

	private readonly SemaphoreSlim _locker = new(1, 1);
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private Task? _backgroundTask;
	private int _expiry = options.Value.ChannelExpiry;
	private Timer? _timer;
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

			_timer = new Timer(_ =>
			{
				_ = Interlocked.Decrement(ref _expiry);

				if (_expiry <= 0)
					Dispose();
			}, null, 0, 1000);

			_backgroundTask = Task.Run(
				() => ProcessCommandLoopAsync(_cancellationTokenSource.Token),
				_cancellationTokenSource.Token);
		}
		finally
		{
			_ = _locker.Release();
		}
	}

	public async ValueTask PushCommandAsync(GameBetWorkerRequest request, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(
			token1: cancellationToken,
			token2: _cancellationTokenSource.Token);

		await _channel.Writer.WriteAsync(
			item: request,
			cancellationToken: cts.Token).ConfigureAwait(false);
	}

	public void ExtendExpiry()
	{
		_ = Interlocked.Exchange(ref _expiry, options.Value.ChannelExpiry);
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
				_timer?.Dispose();
				_timer = null;

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
			if (reader.TryRead(out var request))
			{
				using var activity = _ActivitySource.StartActivity(
					"GameBetsWorkerChannel handle exchange",
					 ActivityKind.Internal,
					parentContext: default);
				try
				{
					logger.LogInformation("{logInformation}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Key,
							Data = new
							{
								request.GameProviderId,
								request.GameId,
								request.PlayerCode,
								request.WalletId,
								request.ExternalRoundId,
								request.ExternalBetId,
								ExternalCreatedOn = request.ExternalCreatedOn.ToString("u")
							},
							Message = "This processing."
						}));
					using var scope = serviceProvider.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
					await mediator.Send(
						request: request,
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "{logError}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Key,
							Data = new
							{
								request.GameProviderId,
								request.GameId,
								request.PlayerCode,
								request.WalletId,
								request.ExternalRoundId,
								request.ExternalBetId,
								ExternalCreatedOn = request.ExternalCreatedOn.ToString("u")
							},
							Message = "This exception occurs."
						}));
				}
			}
		}
	}
}
