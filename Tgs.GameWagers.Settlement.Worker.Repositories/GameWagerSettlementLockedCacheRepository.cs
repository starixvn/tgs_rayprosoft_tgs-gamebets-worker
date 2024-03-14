using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.Models;

namespace Tgs.GameWagers.Settlement.Worker.Repositories;
internal class GameWagerSettlementLockedCacheRepository(
	IGameWagerSettlementRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<GameWagerSettlementLockedCacheRepository> logger) : IGameWagerSettlementRepository
{
	public async ValueTask AddAsync(GameWagerSettlementAddModel source, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await SetKeyAsync(
				gameWagerPrimaryId: source.GameWagerPrimaryId,
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
			logger.LogError(ex, "This Game Wager Settlement First add exception occurs");
			await DeleteKeyAsync(
				gameWagerPrimaryId: source.GameWagerPrimaryId)
				.ConfigureAwait(false);
		}
	}
	public async ValueTask<bool> ExistAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		if (!await ExistsKeyAsync(
			gameWagerPrimaryId: gameWagerPrimaryId)
			.ConfigureAwait(false))
		{
			var exist = await repository.ExistAsync(
				gameWagerPrimaryId: gameWagerPrimaryId,
				externalCreatedOn: externalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (exist)
			{
				await SetKeyAsync(
					gameWagerPrimaryId: gameWagerPrimaryId,
					externalCreatedOn: externalCreatedOn)
					.ConfigureAwait(false);
			}

			return exist;
		}

		return true;
	}
	public ValueTask<GameWagerSettlementInfoModel?> GetAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameWagerPrimaryId, externalCreatedOn, cancellationToken);
	public ValueTask UpdateAsync(GameWagerSettlementUpdateModel source, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(source, cancellationToken);

	private async ValueTask<bool> ExistsKeyAsync(Guid gameWagerPrimaryId)
		=> await database.KeyExistsAsync(
			key: GetKey(gameWagerPrimaryId))
		.ConfigureAwait(false);

	private async ValueTask<bool> SetKeyAsync(Guid gameWagerPrimaryId, DateTimeOffset externalCreatedOn)
		=> await database.StringSetAsync(
			key: GetKey(gameWagerPrimaryId),
			value: new JsonObject
			{
				["GameWagerPrimaryId"] = gameWagerPrimaryId,
				["ExternalCreatedOn"] = externalCreatedOn,
				["LockedOn"] = DateTimeOffset.UtcNow

			}.ToJsonString(),
			when: When.NotExists,
			expiry: expiry)
		.ConfigureAwait(false);

	private async ValueTask DeleteKeyAsync(Guid gameWagerPrimaryId)
		=> await database.KeyDeleteAsync(
			key: GetKey(gameWagerPrimaryId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	private static string GetKey(Guid gameWagerPrimaryId)
		=> string.Concat(typeof(GameWagerSettlementLockedCacheRepository).Namespace!.Replace('.', '-').ToLower(), "settlement", ":", "locked", gameWagerPrimaryId);
}
