using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

internal class GameBetsWorkerDispatcher(IServiceProvider serviceProvider) : IGameBetsWorkerDispatcher, IDisposable, IAsyncDisposable
{
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private readonly ConcurrentDictionary<string, GameBetsWorkerChannel> _counters = new();
	private bool _disposedValue;

	~GameBetsWorkerDispatcher()
	{
		Dispose(false);
	}

	public async ValueTask PushCommandAsync(GameBetWorkerCommand command, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

		var counter = _counters.GetOrAdd(
			string.Concat(command.GameProviderCode, "_", command.PageIndex),
			key =>
			{
				var scope = serviceProvider.CreateScope();
				var newCounter = ActivatorUtilities.CreateInstance<GameBetsWorkerChannel>(
					scope.ServiceProvider,
					key);

				newCounter.Disposed += (_, _) =>
				{
					_ = _counters.TryRemove(newCounter.GameProviderCode, out _);
					scope.Dispose();
				};

				return newCounter;
			});
		await counter.PrepareAsync(cts.Token).ConfigureAwait(false);
		await counter.PushCommandAsync(command, cts.Token).ConfigureAwait(false);
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
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}
}
