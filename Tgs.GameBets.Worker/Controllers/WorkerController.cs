using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Controllers;

[Route("[controller]")]
[ApiController]
public class WorkerController : ControllerBase
{
	/// <summary>
	/// Handles the asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"gameBetId": "0191270f-5d08-0000-0001-07e83dcd089c",
	///				"gameProviderId": "d1e79871-363b-7776-213f-84ba6026e03c",
	///				"gameProviderConfigurationId": "a8f6819f-72ae-a4e4-2e6a-34f1cbeaf96a",
	///				"gameId": 2000015,
	///				"resellerId": "b5a2c708-ab70-4316-9f4d-5d2ad0e77460",
	///				"licenseeId": "66c5fef5-217b-4929-9a08-72c2479a8f4e",
	///				"brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
	///				"playerCode": "4un4b9l34304",
	///				"currencyCode": "CNY",
	///				"walletId": "58ab70d2-71cd-4793-9807-82b1e0e46eaa",
	///				"externalRoundId": "888-1722937124-92240583-21",
	///				"externalBetId": "888-1722937124-92240583-21",
	///				"externalCreatedOn": "2024-08-06T17:38:44+08:00",
	///				"externalClosedOn": "2024-08-06T17:39:03+08:00",
	///				"externalPayoffOn": "2024-08-06T17:39:03+08:00",
	///				"externalResettleOn": "1970-01-01T00:00:00+00:00",
	///				"betState": "Win",
	///				"betType": "Normal",
	///				"freeGameState": "None",
	///				"gambleState": "None",
	///				"cashoutState": "None",
	///				"resettleState": "None",
	///				"jackpotState": "None",
	///				"currencyRatio": 1,
	///				"bet": 2,
	///				"validBet": 2,
	///				"settle": 4,
	///				"commission": 0,
	///				"turnover": 2,
	///				"winLoss": 2,
	///				"jackpot": 0,
	///				"additionalData": "{\"AllBet\": 2, \"GameID\": \"888-1722937124-92240583-21\", \"KindID\": 910, \"Profit\": 2, \"ChairID\": 0, \"Revenue\": 0, \"TableID\": 182020001, \"Accounts\": \"200995_4un4b9l34304\", \"LineCode\": \"200995_GPI\", \"ServerID\": 9101, \"CardValue\": \"0c32050b06001\", \"CellScore\": 2, \"ChannelID\": 200995, \"UserCount\": 1, \"GameEndTime\": \"2024-08-06T17:39:03+08:00\", \"GameStartTime\": \"2024-08-06T17:38:44+08:00\"}"
	///			}
	/// </remarks>
	[HttpPost]
	public async ValueTask HandleAsync(
		[FromServices] IMessageSender messageSender,
		[FromBody] Common.ViewModels.WorkerHandleViewModel source)
	{
		var tasks = new Task[]{
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetsWorkerRequest,
				data: new Common.Protos.GameBetsWorkerRequest
				{
					GameBetId = source.BrandId,
					GameProviderId = source.GameProviderId,
					GameProviderConfigurationId = source.GameProviderConfigurationId,
					GameId = source.GameId,
					ResellerId = source.ResellerId,
					LicenseeId = source.LicenseeId,
					BrandId = source.BrandId,
					PlayerCode = source.PlayerCode,
					CurrencyCode = source.CurrencyCode,
					WalletId = source.WalletId,
					ExternalRoundId = source.ExternalRoundId,
					ExternalBetId = source.ExternalBetId,
					ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
					ExternalClosedOn = source.ExternalClosedOn.ToTimestamp(),
					ExternalPayoffOn = source.ExternalPayoffOn.ToTimestamp(),
					ExternalResettleOn = source.ExternalResettleOn.ToTimestamp(),
					BetState = source.BetState,
					BetType = source.BetType,
					FreeGameState = source.FreeGameState,
					GambleState = source.GambleState,
					CashoutState = source.CashoutState,
					ResettleState = source.ResettleState,
					JackpotState = source.JackpotState,
					CurrencyRatio = source.CurrencyRatio,
					Bet = source.Bet,
					ValidBet = source.ValidBet,
					Settle = source.Settle,
					Commission = source.Commission,
					Turnover = source.Turnover,
					WinLoss = source.WinLoss,
					Jackpot = source.Jackpot,
					AdditionalData = source.AdditionalData
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetsWorkerExpiryRequest,
				data: new Common.Protos.GameBetsWorkerExpiryRequest
				{
					GameProviderId = source.GameProviderId,
					GameId = source.GameId,
					BrandId = source.BrandId
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
		};

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}
}
