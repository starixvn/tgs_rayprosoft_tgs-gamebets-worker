using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameBets.Client;
using Tgs.GameBets.Common.ViewModels;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Repositories;

internal class GameBetRepository(
	ILogger<GameBetRepository> logger,
	IOptions<PolicySettingsOptions> options,
	IGameBetsClient client) : IGameBetRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<GameBets.Common.Enums.GameBetCommandResult> CommandAsync(GameBetCommandModel source, CancellationToken cancellationToken = default)
	{
		var gameBetCommand = new GameBetCommandViewModel(
			Id: source.GameBetId,
			GameProviderId: source.GameProviderId,
			GameProviderConfigurationId: source.GameProviderConfigurationId,
			GameId: source.GameId,
			ResellerId: source.ResellerId,
			LicenseeId: source.LicenseeId,
			BrandId: source.BrandId,
			PlayerCode: source.PlayerCode,
			CurrencyCode: source.CurrencyCode,
			WalletId: source.WalletId,
			ExternalRoundId: source.ExternalRoundId,
			ExternalBetId: source.ExternalBetId,
			ExternalCreatedOn: source.ExternalCreatedOn,
			ExternalClosedOn: source.ExternalClosedOn,
			ExternalPayoffOn: source.ExternalPayoffOn,
			ExternalResettleOn: source.ExternalResettleOn,
			BetState: source.BetState,
			BetType: source.BetType,
			FreeGameState: source.FreeGameState,
			GambleState: source.GambleState,
			CashoutState: source.CashoutState,
			ResettleState: source.ResettleState,
			JackpotState: source.JackpotState,
			CurrencyRatio: source.CurrencyRatio,
			Bet: source.Bet,
			ValidBet: source.ValidBet,
			Settle: source.Settle,
			Commission: source.Commission,
			Turnover: source.Turnover,
			WinLoss: source.WinLoss,
			Jackpot: source.Jackpot,
			AdditionalData: source.AdditionalData);

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

		return await retryPolicy.ExecuteAsync(() => client.GameBetCommandAsync(
			source: gameBetCommand,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}
}
