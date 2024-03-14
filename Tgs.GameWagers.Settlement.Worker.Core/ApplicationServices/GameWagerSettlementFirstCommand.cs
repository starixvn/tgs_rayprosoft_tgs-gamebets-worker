namespace Tgs.GameWagers.Settlement.Worker.Core.ApplicationServices;
public record GameWagerSettlementFirstCommand(
	 Guid GameWagerPrimaryId,
	 DateTimeOffset ExternalCreatedOn);
