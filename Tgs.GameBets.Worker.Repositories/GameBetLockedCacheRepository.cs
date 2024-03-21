using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Repositories;
internal class GameBetLockedCacheRepository(
	IGameBetRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameBetLockedCacheRepository> logger) : IGameBetRepository
{
	public async ValueTask AddAsync(GameBetAddModel source, CancellationToken cancellationToken = default)
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
			logger.LogError(ex, "This Game Bet add exception occurs");
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
	public ValueTask<GameBetInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalWagerId, externalCreatedOn, cancellationToken);
	public ValueTask UpdateAsync(GameBetUpdateModel source, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, cancellationToken);

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
		=> string.Concat(typeof(GameBetLockedCacheRepository).Namespace!.Replace('.', '-').ToLower(), "bet", ":", "locked", gameProviderId, ":", gameId, ":", walletId, ":", externalWagerId);
}
