using Tgs.GameBets.Worker.Core.Models;

using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetFirstCommandService(
	IGameBetFirstRepository gameBetFirstRepository,
	IGameBetRepository gameBetRepository,
	ISerialNumberGenerator serialNumberGenerator,
	ICommandService<GameWagerStatCloseCommand> service) : ICommandService<GameBetFirstCommand>
{
	public async ValueTask ExecuteAsync(GameBetFirstCommand command, CancellationToken cancellationToken = default)
	{
		if (command.BetState is Queue.Common.Enums.BetState.Waiting or Queue.Common.Enums.BetState.Running)
		{
			return;
		}

		if (await gameBetFirstRepository.ExistAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			return;
		}

		var model = await gameBetRepository.GetAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(command.ToString());

		var id = await serialNumberGenerator.GenerateAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameBetFirstRepository.AddAsync(
			source: new GameBetFirstAddModel(
				Id: id,
				GameBetId: model.Id,
				GameProviderId: model.GameProviderId,
				GameProviderConfigurationId: model.GameProviderConfigurationId,
				GameId: model.GameId,
				ResellerId: model.ResellerId,
				LicenseeId: model.LicenseeId,
				BrandId: model.BrandId,
				PlayerCode: model.PlayerCode,
				CurrencyCode: model.CurrencyCode,
				WalletId: model.WalletId,
				ExternalRoundId: model.ExternalRoundId,
				ExternalBetId: model.ExternalBetId,
				ExternalCreatedOn: model.ExternalCreatedOn,
				ExternalClosedOn: model.ExternalClosedOn,
				ExternalPayoffOn: model.ExternalPayoffOn,
				ExternalResettleOn: model.ExternalResettleOn,
				BetState: model.BetState,
				BetType: model.BetType,
				FreeGameState: model.FreeGameState,
				GambleState: model.GambleState,
				CashoutState: model.CashoutState,
				ResettleState: model.ResettleState,
				JackpotState: model.JackpotState,
				CurrencyRatio: model.CurrencyRatio,
				Bet: model.Bet,
				ValidBet: model.ValidBet,
				Settle: model.Settle,
				Commission: model.Commission,
				Turnover: model.Turnover,
				WinLoss: model.WinLoss,
				Jackpot: model.Jackpot,
				CreatedOn: model.CreatedOn,
				ClosedOn: model.ClosedOn,
				UpdatedOn: model.UpdatedOn,
				AdditionalData: model.AdditionalData),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await service.ExecuteAsync(
			command: new GameWagerStatCloseCommand(
				model.GameProviderId,
				model.GameId,
				model.PlayerCode,
				model.WalletId,
				model.ExternalRoundId,
				model.ExternalBetId,
				model.ExternalCreatedOn),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
