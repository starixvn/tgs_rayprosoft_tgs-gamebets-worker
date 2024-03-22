using Tgs.GameBets.Worker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;
internal class GameWagerStatCloseCommandService(
	IStatisticalTimeUtnsil utnsil,
	IGameWagerStatCloseRepository repository) : ICommandService<GameWagerStatCloseCommand>
{
	public ValueTask ExecuteAsync(GameWagerStatCloseCommand command, CancellationToken cancellationToken = default)
	{
		var statOn = utnsil.GetStatOn(command.ClosedOn);

		return repository.AddAsync(
			source: new GameWagerStatCloseAdd(
				GameProviderId: command.GameProviderId,
				GameId: command.GameId,
				ResellerId: command.ResellerId,
				LicenseeId: command.LicenseeId,
				BrandId: command.BrandId,
				PlayerCode: command.PlayerCode,
				WalletId: command.WalletId,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				StatOn: statOn,
				Bet: command.Bet,
				ValidBet: command.ValidBet,
				Settle: command.Settle,
				Commission: command.Commission,
				Turnover: command.Turnover,
				WinLoss: command.WinLoss),
			cancellationToken: cancellationToken);
	}
}
