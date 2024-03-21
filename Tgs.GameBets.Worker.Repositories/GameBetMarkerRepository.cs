using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Tgs.GameBets.Worker.Core;
using Tgs.GameBets.Worker.Core.Models;
using Tgs.GameBets.Worker.Entities;

namespace Tgs.GameBets.Worker.Repositories;
internal class GameBetMarkerRepository(
	TgsContext context) : IGameBetMarkerRepository
{
	public async ValueTask AddAsync(string code, string directions, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMarkers.FirstOrDefaultAsync(
			predicate: gwm => gwm.Code == code,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (entity is not null)
		{
			return;
		}

		entity = new GameBetMarker
		{
			Code = code,
			Marker = Guid.Empty,
			Expansions = new JsonObject().ToJsonString(),
			Directions = directions,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow
		};

		_ = await context.GameBetMarkers.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

	}
	public async ValueTask<GameBetMarkerInfoModel?> GetAsync(string code, CancellationToken cancellationToken = default)
		=> await context.GameBetMarkers
		.Where(predicate: gwm => gwm.Code == code)
		.Select(selector: gwm => new GameBetMarkerInfoModel(
			gwm.Code,
			gwm.Marker,
			gwm.Expansions,
			gwm.Directions))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask UpdateAsync(string code, Guid marker, CancellationToken cancellationToken = default)
	{
		var entity = await context.GameBetMarkers.SingleAsync(
			predicate: gwm => gwm.Code == code,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		entity.Marker = marker;
		entity.UpdatedOn = DateTime.UtcNow;

		_ = context.GameBetMarkers.Update(entity: entity);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
