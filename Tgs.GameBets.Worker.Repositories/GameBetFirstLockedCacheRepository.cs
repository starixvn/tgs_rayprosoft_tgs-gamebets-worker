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
				externalWagerId: source.ExternalWagerId,
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
				externalWagerId: source.ExternalWagerId)
				.ConfigureAwait(false);
		}
	}
	public async ValueTask<bool> ExistAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		if (!await ExistsKeyAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalWagerId: externalWagerId)
			.ConfigureAwait(false))
		{
			var exist = await repository.ExistAsync(
				gameProviderId: gameProviderId,
				gameId: gameId,
				walletId: walletId,
				externalWagerId: externalWagerId,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (exist)
			{
				await SetKeyAsync(
					gameProviderId: gameProviderId,
					gameId: gameId,
					walletId: walletId,
					externalWagerId: externalWagerId,
					externalCreatedOn: externalCreatedOn)
					.ConfigureAwait(false);
			}

			return exist;
		}

		return true;
	}

	private async ValueTask<bool> ExistsKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId)
		=> await database.KeyExistsAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalWagerId))
		.ConfigureAwait(false);

	private async ValueTask<bool> SetKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn)
		=> await database.StringSetAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalWagerId),
			value: new JsonObject
			{
				["GameProviderId"] = gameProviderId,
				["GameId"] = gameId,
				["WalletId"] = walletId,
				["ExternalWagerId"] = externalWagerId,
				["ExternalCreatedOn"] = externalCreatedOn,
				["LockedOn"] = DateTimeOffset.UtcNow

			}.ToJsonString(),
			when: When.NotExists,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask DeleteKeyAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameProviderId, gameId, walletId, externalWagerId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId)
		=> string.Concat(typeof(GameBetFirstLockedCacheRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "first", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalWagerId);
}
