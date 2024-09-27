namespace Tgs.GameBets.Worker.Client;

public interface IGameBetsWorkerClient
{
	public ValueTask HandleAsync(Common.ViewModels.GameBetHandleViewModel source, CancellationToken cancellationToken = default);
}
