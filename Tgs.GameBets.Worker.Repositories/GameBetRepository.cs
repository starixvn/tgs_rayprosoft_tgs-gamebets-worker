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

	public async ValueTask CommandAsync(GameBetCommandModel source, CancellationToken cancellationToken = default)
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
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn} Exception:{exception} RetryTime:{retryTime} RetryCount:{retryCount} Source:{source}",
					DateTime.Now.ToString("o"),
					exception.Message,
					retryTime,
					retryCount,
					new
					{
						source.GameProviderId,
						source.GameId,
						source.PlayerCode,
						source.WalletId,
						source.ExternalRoundId,
						source.ExternalBetId,
						source.ExternalCreatedOn
					}));

		await retryPolicy.ExecuteAsync(() => client.GameBetCommandAsync(
			source: gameBetCommand,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}

	public async ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn} Exception:{exception} RetryTime:{retryTime} RetryCount:{retryCount} Source:{source}",
					DateTime.Now.ToString("o"),
					exception.Message,
					retryTime,
					retryCount,
					new
					{
						GameProviderId = gameProviderId,
						GameId = gameId,
						WalletId = walletId,
						ExternalBetId = externalBetId,
						ExternalCreatedOn = externalCreatedOn
					}));

		var info = await retryPolicy.ExecuteAsync(() => client.GameBetGetByExternalBetAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId,
			externalCreatedOn: externalCreatedOn,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);

		return info is null
			? null :
			new GameBetInfoModel(
				Id: info.Id,
				GameProviderId: info.GameProviderId,
				GameProviderConfigurationId: info.GameProviderConfigurationId,
				GameId: info.GameId,
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				CurrencyCode: info.CurrencyCode,
				WalletId: info.WalletId,
				ExternalRoundId: info.ExternalRoundId,
				ExternalBetId: info.ExternalBetId,
				ExternalCreatedOn: info.ExternalCreatedOn,
				ExternalClosedOn: info.ExternalClosedOn,
				ExternalPayoffOn: info.ExternalPayoffOn,
				ExternalResettleOn: info.ExternalResettleOn,
				BetState: info.BetState,
				BetType: info.BetType,
				FreeGameState: info.FreeGameState,
				GambleState: info.GambleState,
				CashoutState: info.CashoutState,
				ResettleState: info.ResettleState,
				JackpotState: info.JackpotState,
				CurrencyRatio: info.CurrencyRatio,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss,
				Jackpot: info.Jackpot,
				CreatedOn: info.CreatedOn,
				ClosedOn: info.ClosedOn,
				UpdatedOn: info.UpdatedOn,
				AdditionalData: info.AdditionalData);
	}
}
