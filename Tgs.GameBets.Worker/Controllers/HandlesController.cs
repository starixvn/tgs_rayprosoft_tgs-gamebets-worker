using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Valhalla.MessageQueue;

namespace Tgs.GameBets.Worker.Controllers;

[Route("[controller]")]
[ApiController]
public class HandlesController : ControllerBase
{
	/// <summary>
	/// Games the bet close handle asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///		Post
	///		{
	///			"gameBetId": "0191e3f1-e185-0000-0001-07e8778bb552",
	///			"gameProviderId": "147cef56-4fcd-4ee3-b748-87ad398f2871",
	///			"gameProviderConfigurationId": "dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a",
	///			"gameId": 6700018,
	///			"resellerId": "b5a2c708-ab70-4316-9f4d-5d2ad0e77460",
	///			"licenseeId": "66c5fef5-217b-4929-9a08-72c2479a8f4e",
	///			"brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
	///			"playerCode": "4unykr72x8d9",
	///			"currencyCode": "CNY",
	///			"walletId": "8bff49a2-66ff-4c66-81f6-c9b242f2b868",
	///			"externalRoundId": "6323442723",
	///			"externalBetId": "6323442723",
	///			"externalCreatedOn": "2024-09-12T09:55:12.737+08:00",
	///			"externalClosedOn": "2024-09-12T09:55:15.555+08:00",
	///			"externalPayoffOn": "2024-09-12T09:55:15.555+08:00",
	///			"externalResettleOn": "1970-01-01T08:00:00+08:00",
	///			"betState": "Win",
	///			"betType": "Normal",
	///			"freeGameState": "None",
	///			"gambleState": "None",
	///			"cashoutState": "None",
	///			"resettleState": "None",
	///			"jackpotState": "None",
	///			"currencyRatio": 1,
	///			"bet": 0.8,
	///			"validBet": 0.8,
	///			"settle": 1.2,
	///			"winLoss": 0.4,
	///			"turnover": 0.8,
	///			"commission": 0.01,
	///			"contribute": 0.001,
	///			"jackpot": 0,
	///			"createdOn": "2024-09-12T09:56:05+08:00",
	///			"closedOn": "2024-09-12T09:56:05+08:00",
	///			"additionalData": {},
	///			"extendedData": {}
	///		}
	/// </remarks>
	[HttpPost("close")]
	public async ValueTask GameBetCloseHandleAsync(
		[FromServices] IMessageSender messageSender,
		[FromBody] Common.ViewModels.GameBetCloseHandleViewModel source)
	{
		var tasks = new Task[]{
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetCloseWorkerRequest,
				data: new Common.Protos.GameBetCloseWorkerRequest
				{
					GameBetId = source.GameBetId,
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
					WinLoss = source.WinLoss,
					Turnover = source.Turnover,
					Commission = source.Commission,
					Contribute = source.Contribute,
					Jackpot = source.Jackpot,
					CreatedOn=source.CreatedOn.ToTimestamp(),
					ClosedOn=source.ClosedOn.ToTimestamp(),
					AdditionalData = source.AdditionalData.ToJsonString(),
					ExtendedData = source.ExtendedData.ToJsonString(),
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetCloseWorkerExpiryRequest,
				data: new Common.Protos.GameBetCloseWorkerExpiryRequest
				{
					GameProviderId = source.GameProviderId,
					GameId = source.GameId,
					BrandId = source.BrandId
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
		};

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}

	/// <summary>
	/// Games the bet running handle asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///		Post
	///		{
	///			"gameBetId": "0191e3f1-e185-0000-0001-07e8778bb552",
	///			"gameProviderId": "147cef56-4fcd-4ee3-b748-87ad398f2871",
	///			"gameProviderConfigurationId": "dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a",
	///			"gameId": 6700018,
	///			"resellerId": "b5a2c708-ab70-4316-9f4d-5d2ad0e77460",
	///			"licenseeId": "66c5fef5-217b-4929-9a08-72c2479a8f4e",
	///			"brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
	///			"playerCode": "4unykr72x8d9",
	///			"currencyCode": "CNY",
	///			"walletId": "8bff49a2-66ff-4c66-81f6-c9b242f2b868",
	///			"externalRoundId": "6323442723",
	///			"externalBetId": "6323442723",
	///			"externalCreatedOn": "2024-09-12T09:55:12.737+08:00",
	///			"betState": "Win",
	///			"betType": "Normal",
	///			"currencyRatio": 1,
	///			"bet": 0.8,
	///			"validBet": 0.8,
	///			"createdOn": "2024-09-12T09:56:05+08:00",
	///			"additionalData": {},
	///			"extendedData": {}
	///		}
	/// </remarks>
	[HttpPost("running")]
	public async ValueTask GameBetRunningHandleAsync(
		[FromServices] IMessageSender messageSender,
		[FromBody] Common.ViewModels.GameBetRunningHandleViewModel source)
	{
		var tasks = new Task[]{
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetRunningWorkerRequest,
				data: new Common.Protos.GameBetRunningWorkerRequest
				{
					GameBetId = source.GameBetId,
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
					BetState = source.BetState,
					BetType = source.BetType,
					CurrencyRatio = source.CurrencyRatio,
					Bet = source.Bet,
					ValidBet = source.ValidBet,
					CreatedOn=source.CreatedOn.ToTimestamp(),
					AdditionalData = source.AdditionalData.ToJsonString(),
					ExtendedData = source.ExtendedData.ToJsonString(),
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameBetsWorkerNatsConst.GameBetRunningWorkerExpiryRequest,
				data: new Common.Protos.GameBetRunningWorkerExpiryRequest
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
