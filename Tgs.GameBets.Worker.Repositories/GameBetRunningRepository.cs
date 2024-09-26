using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameBets.Client;
using Tgs.GameBets.Common.ViewModels;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Repositories;

internal class GameBetRunningRepository(
	ILogger<GameBetRepository> logger,
	IOptions<PolicySettingsOptions> options,
	IGameBetsClient client) : IGameBetRunningRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask CommandAsync(GameBetRunningCommand source, CancellationToken cancellationToken = default)
	{
		var gameBetRunningCommand = new GameBetRunningCommandViewModel
		{
			GameBetId = source.GameBetId,
			GameProviderId = source.GameProviderId,
			GameProviderConfigurationId = source.GameProviderConfigurationId,
			GameId = source.GameId,
			ResellerId = source.ResellerId,
			LicenseeId = source.LicenseeId,
			BrandId = source.BrandId,
			PlayerCode = source.PlayerCode,
			CurrencyCode = source.CurrencyCode,
			WalletId = source.WalletId,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalCreatedOn = source.ExternalCreatedOn,
			BetType = source.BetType,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			CreatedOn = source.CreatedOn,
			AdditionalData = source.AdditionalData,
			ExtendedData = source.ExtendedData
		};

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							source.GameProviderId,
							source.GameId,
							source.PlayerCode,
							source.WalletId,
							source.ExternalRoundId,
							source.ExternalBetId,
							ExternalCreatedOn = source.ExternalCreatedOn.ToString("u")
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.GameBetRunningCommandAsync(
		   source: gameBetRunningCommand,
		   cancellationToken: cancellationToken).AsTask())
		   .ConfigureAwait(false);
	}

	public async ValueTask DeleteAsync(Guid gameBetId, CancellationToken cancellationToken = default)
	{
		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							GameBetId = gameBetId
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.GameBetRunningDeleteAsync(
		   gameBetId: gameBetId,
		   cancellationToken: cancellationToken).AsTask())
		   .ConfigureAwait(false);
	}
}
