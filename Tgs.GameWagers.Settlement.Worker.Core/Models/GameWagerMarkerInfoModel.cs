namespace Tgs.GameWagers.Settlement.Worker.Core.Models;
public record GameWagerMarkerInfoModel(
	string Code,
	Guid Marker,
	string Expansions,
	string Directions);
