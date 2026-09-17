using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface IReviewRepository
{
    Task<bool> UserHasReviewedAsync(Guid productId, Guid userId, CancellationToken ct = default);
    Task<PagedResult<Review>> GetForProductAsync(Guid productId, int page, int pageSize, CancellationToken ct = default);
    Task<(int Count, double AverageRating)> GetRatingStatsAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(Review review, CancellationToken ct = default);

    // بيرجع كل التقييمات كـtext بس (للـAI Review Analyzer، Phase 6) — مفيش pagination هنا عمدًا
    Task<IReadOnlyList<string>> GetCommentsForProductAsync(Guid productId, int maxCount, CancellationToken ct = default);
}
