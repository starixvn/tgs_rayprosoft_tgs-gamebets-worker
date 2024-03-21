namespace Tgs.GameBets.Worker.Core.Models;
public record GameBetMarkerInfoModel(
	string Code,
	Guid Marker,
	string Expansions,
	string Directions);
