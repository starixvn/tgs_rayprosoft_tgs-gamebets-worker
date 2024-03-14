using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Settlement.Worker.Core;
using Tgs.GameWagers.Settlement.Worker.Core.Models;
using Tgs.GameWagers.Settlement.Worker.Entities;

namespace Tgs.GameWagers.Settlement.Worker.Repositories;
internal class GameWagerMarkerRepository(
	TgsContext context) : IGameWagerMarkerRepository
{
	public async ValueTask AddAsync(string code, string directions, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameWagerMarkers.FirstOrDefaultAsync(
			predicate: gwm => gwm.Code == code,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (entity is not null)
		{
			return;
		}

		entity = new GameWagerMarker
		{
			Code = code,
			Marker = Guid.Empty,
			Expansions = new JsonObject().ToJsonString(),
			Directions = directions,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow
		};

		_ = await context.GameWagerMarkers.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

	}
	public async ValueTask<GameWagerMarkerInfoModel?> GetAsync(string code, CancellationToken cancellationToken = default)
		=> await context.GameWagerMarkers
		.Where(predicate: gwm => gwm.Code == code)
		.Select(selector: gwm => new GameWagerMarkerInfoModel(
			gwm.Code,
			gwm.Marker,
			gwm.Expansions,
			gwm.Directions))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask UpdateAsync(string code, Guid marker, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameWagerMarkers.SingleAsync(
			predicate: gwm => gwm.Code == code,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		entity.Marker = marker;
		entity.UpdatedOn = DateTime.UtcNow;

		_ = context.GameWagerMarkers.Update(entity: entity);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
