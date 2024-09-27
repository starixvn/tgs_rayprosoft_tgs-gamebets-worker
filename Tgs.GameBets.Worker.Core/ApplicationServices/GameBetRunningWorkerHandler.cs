using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class GameBetRunningWorkerHandler(
	ILogger<GameBetRunningWorkerHandler> logger,
	IGameBetRunningRepository repository) : IRequestHandler<GameBetRunningWorkerRequest>
{
	public Task Handle(GameBetRunningWorkerRequest request, CancellationToken cancellationToken)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					request.GameProviderId,
					request.GameId,
					request.PlayerCode,
					request.WalletId,
					request.ExternalRoundId,
					request.ExternalBetId,
					ExternalCreatedOn = request.ExternalCreatedOn.ToString("u"),
					request.BetState
				}
			}));
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
