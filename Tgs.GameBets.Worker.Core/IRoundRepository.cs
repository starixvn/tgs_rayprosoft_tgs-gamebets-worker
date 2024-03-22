using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IRoundRepository
{
	ValueTask AddAsync(RoundAddModel source, CancellationToken cancellationToken = default);

	ValueTask ClosedAsync(Guid id, RoundClosedModel source, CancellationToken cancellationToken = default);

	ValueTask<RoundInfoModel?> GetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<RoundInfoModel?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default);

	ValueTask OpenAsync(Guid id, RoundOpenModel source, CancellationToken cancellationToken = default);

	ValueTask ReopenAsync(Guid id, RoundReopenModel source, CancellationToken cancellationToken = default);
}
