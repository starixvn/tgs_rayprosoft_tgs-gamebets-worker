using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetRunningWorkerService(
	ILogger<GameBetRunningWorkerService> logger,
	IGameBetRunningRepository repository) : IRequestHandler<GameBetRunningWorkerResponse>
{
	public Task Handle(GameBetRunningWorkerResponse request, CancellationToken cancellationToken)
	{
		return request.BetState is GameBets.Common.Enums.BetState.Waiting or GameBets.Common.Enums.BetState.Running
			? repository.CommandAsync(
				source: new GameBetRunningCommand(
					GameBetId: request.GameBetId,
					GameProviderId: request.GameProviderId,
					GameProviderConfigurationId: request.GameProviderConfigurationId,
					GameId: request.GameId,
					ResellerId: request.ResellerId,
					LicenseeId: request.LicenseeId,
					BrandId: request.BrandId,
					PlayerCode: request.PlayerCode,
					CurrencyCode: request.CurrencyCode,
					WalletId: request.WalletId,
					ExternalRoundId: request.ExternalRoundId,
					ExternalBetId: request.ExternalBetId,
					ExternalCreatedOn: request.ExternalCreatedOn,
					BetType: request.BetType,
					CurrencyRatio: request.CurrencyRatio,
					Bet: request.Bet,
					ValidBet: request.ValidBet,
					CreatedOn: request.CreatedOn,
					AdditionalData: request.AdditionalData,
					ExtendedData: request.ExtendedData),
				cancellationToken).AsTask()
			: repository.DeleteAsync(
				gameBetId: request.GameBetId,
				cancellationToken).AsTask();
	}
}
