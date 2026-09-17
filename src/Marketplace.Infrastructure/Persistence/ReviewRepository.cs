using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserHasReviewedAsync(Guid productId, Guid userId, CancellationToken ct = default) =>
        await _context.Reviews.AnyAsync(r => r.ProductId == productId && r.UserId == userId, ct);

    public async Task<PagedResult<Review>> GetForProductAsync(Guid productId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<Review>(items, page, pageSize, totalCount);
    }

    public async Task<(int Count, double AverageRating)> GetRatingStatsAsync(Guid productId, CancellationToken ct = default)
    {
        var query = _context.Reviews.Where(r => r.ProductId == productId && r.IsApproved);

        var count = await query.CountAsync(ct);
        if (count == 0) return (0, 0);

        var avg = await query.AverageAsync(r => r.Rating, ct);
        return (count, avg);
    }

    public async Task AddAsync(Review review, CancellationToken ct = default)
    {
        await _context.Reviews.AddAsync(review, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetCommentsForProductAsync(Guid productId, int maxCount, CancellationToken ct = default) =>
        await _context.Reviews
            .Where(r => r.ProductId == productId && r.IsApproved && r.Comment != null)
            .OrderByDescending(r => r.CreatedAt)
            .Take(maxCount)
            .Select(r => r.Comment!)
            .ToListAsync(ct);
}
