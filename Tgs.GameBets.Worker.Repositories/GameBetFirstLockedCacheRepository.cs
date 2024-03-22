using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameBets.Worker.Repositories;

namespace Tgs.GameBets.Worker.Repositories;
internal class GameBetFirstLockedCacheRepository(
	IGameBetFirstRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetFirstLockedCacheRepository> logger) : IGameBetFirstRepository
{
	public async ValueTask AddAsync(GameBetFirstAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await SetKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId,
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
			logger.LogError(ex, "This Game Bet First add exception occurs");
			await DeleteKeyAsync(
				gameProviderId: source.GameProviderId,
				gameId: source.GameId,
				walletId: source.WalletId,
				externalBetId: source.ExternalBetId)
				.ConfigureAwait(false);
		}
	}
	public async ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		if (!await ExistsKeyAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalBetId: externalBetId)
			.ConfigureAwait(false))
		{
			var exist = await repository.ExistAsync(
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalBetId: externalBetId,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (exist)
			{
				await SetKeyAsync(
					gameProviderId: gameProviderId,
					gameId: gameId,
					walletId: walletId,
					externalBetId: externalBetId,
					externalCreatedOn: externalCreatedOn)
					.ConfigureAwait(false);
			}

			return exist;
		}

		return true;
	}

	private async ValueTask<bool> ExistsKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyExistsAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId))
		.ConfigureAwait(false);

	private async ValueTask<bool> SetKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId, DateTimeOffset externalCreatedOn)
		=> await database.StringSetAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			value: new JsonObject
			{
				["GameProviderId"] = gameProviderId,
				["GameId"] = gameId,
				["WalletId"] = walletId,
				["ExternalBetId"] = externalBetId,
				["ExternalCreatedOn"] = externalCreatedOn,
				["LockedOn"] = DateTimeOffset.UtcNow

			}.ToJsonString(),
			when: When.NotExists,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask DeleteKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalBetId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalBetId)
		=> string.Concat(typeof(GameBetFirstLockedCacheRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "first", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalBetId);
}
