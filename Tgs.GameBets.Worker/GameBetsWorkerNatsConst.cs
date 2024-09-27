namespace Tgs.GameBets.Worker;

public record GameBetsWorkerNatsConst
{
	public const string GameBetWorkerExpiryRequest = "game.gamebet.worker.expiry.request";
	public const string GameBetCloseWorkerRequest = "game.gamebet.close.worker.request";
	public const string GameBetRunningWorkerRequest = "game.gamebet.running.worker.request";
}
