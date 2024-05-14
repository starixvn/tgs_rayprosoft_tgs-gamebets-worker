namespace Tgs.GameBets.Worker.Common;

public record GameBetsWorkerNatsConst
{
	public const string GameBetsWorkerRequest = "game.gamebet.worker.request";
	public const string GameBetsWorkerPublish = "game.gamebet.worker.publish";
	public const string GameBetsWorkerAddPublish = "game.gamebet.worker.add.publish";
	public const string GameBetsWorkerUpdatePublish = "game.gamebet.worker.update.publish";
}
