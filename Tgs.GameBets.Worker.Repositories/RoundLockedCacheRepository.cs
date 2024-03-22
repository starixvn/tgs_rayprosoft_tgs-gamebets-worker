using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Repositories;
internal class RoundLockedCacheRepository(
	IRoundRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<RoundLockedCacheRepository> logger) : IRoundRepository
{
	public async ValueTask AddAsync(RoundAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await SetKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalRoundId: source.ExternalRoundId,
				externalCreatedOn: source.ExternalCreatedOn)
				.ConfigureAwait(false))
			{
				return;
			}

			await repository.AddAsync(
				source: source,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "This Game Bet add exception occurs");
			await DeleteKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalRoundId: source.ExternalRoundId)
				.ConfigureAwait(false);
		}
	}
	public ValueTask ClosedAsync(Guid id, RoundClosedModel source, CancellationToken cancellationToken = default)
		=> repository.ClosedAsync(id, source, cancellationToken);
	public ValueTask<RoundInfoModel?> GetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, externalCreatedOn, cancellationToken);
	public ValueTask<RoundInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalRoundId, externalCreatedOn, cancellationToken);
	public ValueTask<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetIdAsync(gameProviderId, gameId, walletId, externalRoundId, externalCreatedOn, cancellationToken);
	public ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default)
		=> repository.MergeDetailAsync(id, additionalData, cancellationToken);
	public ValueTask OpenAsync(Guid id, RoundOpenModel source, CancellationToken cancellationToken = default)
		=> repository.OpenAsync(id, source, cancellationToken);
	public ValueTask ReopenAsync(Guid id, RoundReopenModel source, CancellationToken cancellationToken = default)
		=> repository.ReopenAsync(id, source, cancellationToken);

	private async ValueTask<bool> SetKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn)
		=> await database.StringSetAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalRoundId),
			value: new JsonObject
			{
				["GameProviderId"] = gameProviderId,
				["GameId"] = gameId,
				["WalletId"] = walletId,
				["ExternalRoundId"] = externalRoundId,
				["ExternalCreatedOn"] = externalCreatedOn,
				["LockedOn"] = DateTimeOffset.UtcNow

			}.ToJsonString(),
			when: When.NotExists,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask DeleteKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalRoundId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId)
		=> string.Concat(typeof(RoundLockedCacheRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalRoundId);
}
