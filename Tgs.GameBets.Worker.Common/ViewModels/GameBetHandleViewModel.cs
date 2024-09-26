using System.ComponentModel;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Tgs.GameBets.Worker.Common.ViewModels;

public record GameBetHandleViewModel
{
	[Required]
	[DefaultValue(typeof(Guid), "0191e3f1-e185-0000-0001-07e8778bb552")]
	public Guid GameBetId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "147cef56-4fcd-4ee3-b748-87ad398f2871")]
	public Guid GameProviderId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a")]
	public Guid GameProviderConfigurationId { get; init; }
	[Required]
	[DefaultValue(typeof(int), "6700018")]
	public int GameId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "b5a2c708-ab70-4316-9f4d-5d2ad0e77460")]
	public Guid ResellerId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "66c5fef5-217b-4929-9a08-72c2479a8f4e")]
	public Guid LicenseeId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "4e86719f-243f-46f6-b010-5c758b7285af")]
	public Guid BrandId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "4unykr72x8d9")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "CNY")]
	public string CurrencyCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(Guid), "8bff49a2-66ff-4c66-81f6-c9b242f2b868")]
	public Guid WalletId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "6323442723")]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "6323442723")]
	public string ExternalBetId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:12.737 +0800")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:15.555 +0800")]
	public DateTimeOffset ExternalClosedOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12 09:55:15.555 +0800")]
	public DateTimeOffset ExternalPayoffOn { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "1970-01-01 08:00:00.000 +0800")]
	public DateTimeOffset ExternalResettleOn { get; init; }
	[Required]
	[DefaultValue(typeof(string), "Win")]
	public Protos.BetState BetState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "Normal")]
	public Protos.BetType BetType { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Protos.FreeGameState FreeGameState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Protos.GambleState GambleState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Protos.CashoutState CashoutState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Protos.ResettleState ResettleState { get; init; }
	[Required]
	[DefaultValue(typeof(string), "None")]
	public Protos.JackpotState JackpotState { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "1.0")]
	public decimal CurrencyRatio { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal Bet { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal ValidBet { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "1.2")]
	public decimal Settle { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "0.4")]
	public decimal WinLoss { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.8")]
	public decimal Turnover { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.01")]
	public decimal Commission { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.001")]
	public decimal Contribute { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0.0")]
	public decimal Jackpot { get; init; }
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12T09:56:05+08:00")]
	public DateTimeOffset CreatedOn { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(DateTimeOffset), "2024-09-12T09:56:05+08:00")]
	public DateTimeOffset ClosedOn { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "{}")]
	public JsonNode AdditionalData { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "{}")]
	public JsonNode ExtendedData { get; init; } = default!;
}
