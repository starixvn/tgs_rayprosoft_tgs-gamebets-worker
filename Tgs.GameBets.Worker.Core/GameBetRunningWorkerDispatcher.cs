using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GameBets.Worker.Core.ApplicationServices;

namespace Tgs.GameBets.Worker.Core;

internal class GameBetRunningWorkerDispatcher(IServiceProvider serviceProvider) : IGameBetRunningWorkerDispatcher, IDisposable, IAsyncDisposable
{
	private readonly ConcurrentDictionary<string, GameBetRunningWorkerChannel> _counters = new();
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private bool _disposedValue;

	~GameBetRunningWorkerDispatcher()
	{
		Dispose(false);
	}

	public async ValueTask PushCommandAsync(GameBetRunningWorkerRequest request, CancellationToken cancellationToken = default)
	{
		var key = GetKey(request.BrandId, request.GameProviderId, request.GameId);
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
		var counter = _counters.GetOrAdd(
			key,
			key =>
			{
				var scope = serviceProvider.CreateScope();
				var newCounter = ActivatorUtilities.CreateInstance<GameBetRunningWorkerChannel>(
					scope.ServiceProvider,
					key);

				newCounter.Disposed += (_, _) =>
				{
					_ = _counters.TryRemove(newCounter.Key, out _);
					scope.Dispose();
				};

				return newCounter;
			});

		await counter.PrepareAsync(cts.Token).ConfigureAwait(false);
		await counter.PushCommandAsync(request, cts.Token).ConfigureAwait(false);
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

	public void ExtendExpiry(Guid brandId, Guid gameProviderId, int gameId, CancellationToken cancellationToken = default)
	{
		var key = GetKey(brandId, gameProviderId, gameId);
		if (_counters.TryGetValue(key, out var counter))
			counter.ExtendExpiry();
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

	private static string GetKey(Guid brandId, Guid gameProviderId, int gameId)
		=> string.Concat("BrandId", "=", brandId, ":", "GameProviderId", "=", gameProviderId, ":", "GameId", "=", gameId);
}
