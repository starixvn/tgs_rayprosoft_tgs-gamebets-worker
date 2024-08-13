using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameBets.Worker.Common.ViewModels;

public record WorkerHandleViewModel
{
	[Required]
	[DefaultValue(type: typeof(Guid), value: "0191270f-5d08-0000-0001-07e83dcd089c")]
	public Guid GameBetId { get; init; }
	[Required]
	[DefaultValue(type: typeof(Guid), value: "d1e79871-363b-7776-213f-84ba6026e03c")]
	public Guid GameProviderId { get; init; }
	[Required]
	[DefaultValue(type: typeof(Guid), value: "a8f6819f-72ae-a4e4-2e6a-34f1cbeaf96a")]
	public Guid GameProviderConfigurationId { get; init; }
	[Required]
	[DefaultValue(type: typeof(int), value: "2000015")]
	public int GameId { get; init; }
	[Required]
	[DefaultValue(type: typeof(Guid), value: "b5a2c708-ab70-4316-9f4d-5d2ad0e77460")]
	public Guid ResellerId { get; init; }
	[Required]
	[DefaultValue(type: typeof(Guid), value: "66c5fef5-217b-4929-9a08-72c2479a8f4e")]
	public Guid LicenseeId { get; init; }
	[Required]
	[DefaultValue(type: typeof(Guid), value: "4e86719f-243f-46f6-b010-5c758b7285af")]
	public Guid BrandId { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "4un4b9l34304")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "CNY")]
	public string CurrencyCode { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(Guid), value: "58ab70d2-71cd-4793-9807-82b1e0e46eaa")]
	public Guid WalletId { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "888-1722937124-92240583-21")]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "888-1722937124-92240583-21")]
	public string ExternalBetId { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:38:44.000+08:00")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:39:03.000+08:00")]
	public DateTimeOffset ExternalClosedOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:39:03.000+08:00")]
	public DateTimeOffset ExternalPayoffOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "1970-01-01T00:00:00.000+00:00")]
	public DateTimeOffset ExternalResettleOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "Win")]
	public Protos.BetState BetState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "Normal")]
	public Protos.BetType BetType { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.FreeGameState FreeGameState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.GambleState GambleState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.CashoutState CashoutState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.ResettleState ResettleState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.JackpotState JackpotState { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "1")]
	public decimal CurrencyRatio { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "2")]
	public decimal Bet { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "2")]
	public decimal ValidBet { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "4")]
	public decimal Settle { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "0")]
	public decimal Commission { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "2")]
	public decimal Turnover { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "2")]
	public decimal WinLoss { get; init; }
	[Required]
	[DefaultValue(type: typeof(decimal), value: "0")]
	public decimal Jackpot { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "{\"AllBet\": 2, \"GameID\": \"888-1722937124-92240583-21\", \"KindID\": 910, \"Profit\": 2, \"ChairID\": 0, \"Revenue\": 0, \"TableID\": 182020001, \"Accounts\": \"200995_4un4b9l34304\", \"LineCode\": \"200995_GPI\", \"ServerID\": 9101, \"CardValue\": \"0c32050b06001\", \"CellScore\": 2, \"ChannelID\": 200995, \"UserCount\": 1, \"GameEndTime\": \"2024-08-06T17:39:03+08:00\", \"GameStartTime\": \"2024-08-06T17:38:44+08:00\"}")]
	public string AdditionalData { get; init; } = default!;
}
