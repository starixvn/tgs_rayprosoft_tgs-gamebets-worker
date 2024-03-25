using Microsoft.Extensions.Logging;
using Tgs.GameBets.Queue.Common.Enums;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameBets.Worker.Core.ApplicationServices;

internal class RoundCommandGetService(
	ILogger<RoundCommandGetService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IRoundRepository repository) : IGetService<RoundCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(RoundCommand query, CancellationToken cancellationToken = default)
	{
		var status = RoundStatus.Open;
		var clientStatus = RoundClientStatus.Open;
		var externalCreatedOn = query.ExternalCreatedOn;
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = query.Bet;
		var validBet = query.ValidBet;
		var settle = 0.0m;
		var commission = 0.0m;
		var turnover = 0.0m;
		var playerWinLoss = 0.0m;

		bool isOperate = false;
		if (query.BetState is not BetState.Waiting and not BetState.Running)
		{
			status = RoundStatus.Closed;
			clientStatus = query.BetState switch
			{
				BetState.Win => RoundClientStatus.Win,
				BetState.Lose => RoundClientStatus.Loss,
				BetState.HalfWin => RoundClientStatus.Win,
				BetState.HalfLose => RoundClientStatus.Loss,
				BetState.Draw => RoundClientStatus.Draw,
				_ => RoundClientStatus.Void,
			};
			externalClosedOn = query.ExternalClosedOn;
			settle = query.Settle;
			commission = query.Commission;
			turnover = query.Turnover;
			playerWinLoss = query.PlayerWinLoss;
		}

		var id = await repository.GetIdAsync(
			gameProviderId: query.GameProviderId,
			externalRoundId: query.ExternalBetId,
			gameId: query.GameId,
			walletId: query.WalletId,
			externalCreatedOn: query.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!id.HasValue)
		{
			id = await serialNumberGenerator.GenerateAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await repository.AddAsync(
				source: new Models.RoundAddModel(
						Id: id.Value,
						GameProviderId: query.GameProviderId,
						ExternalRoundId: query.ExternalBetId,
						GameId: query.GameId,
						PlayerCode: query.PlayerCode,
						WalletId: query.WalletId,
						CurrencyCode: query.CurrencyCode,
						BrandId: query.BrandId,
						GameProviderConfigurationId: query.GameProviderConfigurationId,
						Status: status,
						ClientStatus: clientStatus,
						ExternalCreatedOn: externalCreatedOn,
						ExternalClosedOn: externalClosedOn,
						Bet: bet,
						ValidBet: validBet,
						Settle: settle,
						Commission: commission,
						Turnover: turnover,
						PlayerWinLoss: playerWinLoss),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			isOperate = true;
		}
		else
		{
			logger.LogInformation("LogOn:{logOn} Message:This game round information.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					Id = id.Value,
					query.GameProviderId,
					query.GameId,
					query.PlayerCode,
					query.WalletId,
					ExternalRoundId = query.ExternalBetId,
				});

			var info = await repository.GetAsync(
				id: id.Value,
				externalCreatedOn: query.ExternalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(id.ToString());

			if (info.Status == RoundStatus.Open && status == RoundStatus.Open)
			{
				if (info.Bet != query.Bet ||
					info.ValidBet != query.ValidBet)
				{
					await repository.OpenAsync(
						id: id!.Value,
						source: new Models.RoundOpenModel(
							Bet: query.Bet,
							ValidBet: query.ValidBet),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
				}
			}
			else
			{
				if (info.Status == RoundStatus.Open && status == RoundStatus.Closed)
				{
					await repository.ClosedAsync(
						id: id!.Value,
						source: new Models.RoundClosedModel(
							ClientStatus: clientStatus,
							ExternalClosedOn: externalClosedOn,
							Bet: bet,
							ValidBet: validBet,
							Settle: settle,
							Commission: commission,
							Turnover: turnover,
							PlayerWinLoss: playerWinLoss),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
				}
				else
				{
					if (info.Status == RoundStatus.Closed && status == RoundStatus.Closed)
					{
						if (info.Bet != bet ||
						info.ValidBet != validBet ||
						info.Settle != settle ||
						info.Commission != commission ||
						info.Turnover != turnover ||
						info.PlayerWinLoss != playerWinLoss ||
						info.ClientStatus != clientStatus)
						{
							await repository.ReopenAsync(
								id!.Value,
								source: new Models.RoundReopenModel(
									ClientStatus: clientStatus,
									ExternalClosedOn: externalClosedOn,
									Bet: bet,
									ValidBet: validBet,
									Settle: settle,
									Commission: commission,
									Turnover: turnover,
									PlayerWinLoss: playerWinLoss),
								cancellationToken: cancellationToken)
								.ConfigureAwait(false);
							isOperate = true;
						}
					}
				}
			}
		}

		if (isOperate)
		{
			await repository.MergeDetailAsync(
				id: id.Value,
				additionalData: query.AdditionalData.ToJsonString()!,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return id!.Value;
	}
}
