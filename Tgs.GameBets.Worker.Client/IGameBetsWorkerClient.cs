namespace Tgs.GameBets.Worker.Client;

public interface IGameBetsWorkerClient
{
	public ValueTask HandleAsync(Common.ViewModels.WorkerHandleViewModel source, CancellationToken cancellationToken = default);

	public ValueTask CloseHandleAsync(Common.ViewModels.GameBetCloseHandleViewModel source, CancellationToken cancellationToken = default);

	public ValueTask RunningHandleAsync(Common.ViewModels.GameBetRunningHandleViewModel source, CancellationToken cancellationToken = default);
}
