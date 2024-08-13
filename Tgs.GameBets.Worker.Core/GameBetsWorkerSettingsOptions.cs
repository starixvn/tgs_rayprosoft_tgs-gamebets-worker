namespace Tgs.GameBets.Worker.Core;
public record GameBetsWorkerSettingsOptions
{
	public int ChannelExpiry { get; set; } = 900;
}
