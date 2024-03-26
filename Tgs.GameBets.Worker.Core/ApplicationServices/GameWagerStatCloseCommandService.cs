using Google.Protobuf.WellKnownTypes;
using Tgs.GameBets.Worker.Common;
using Tgs.GameBets.Worker.Common.Protos;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameWagerStatCloseCommandService(
	IMessageSender messageSender) : ICommandService<GameWagerStatCloseCommand>
{
	public ValueTask ExecuteAsync(GameWagerStatCloseCommand command, CancellationToken cancellationToken = default)
	=> messageSender.PublishAsync(
		subject: GameBetsWorkerNatsConst.GameBetsWorkerStat,
		data: new GameBetsWorkerStatMessage
		{
			GameProviderId = command.GameProviderId,
			GameId = command.GameId,
			PlayerCode = command.PlayerCode,
			WalletId = command.WalletId,
			ExternalRoundId = command.ExternalRoundId,
			ExternalBetId = command.ExternalBetId,
			ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp()
		},
		cancellationToken: cancellationToken);
}
