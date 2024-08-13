namespace Tgs.GameBets.Worker.Client;

public interface IGameBetsWorkerClient
{
	public ValueTask HandleAsync(Common.ViewModels.WorkerHandleViewModel source, CancellationToken cancellationToken = default);
}
