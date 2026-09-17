using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishlistItem>> GetForUserAsync(Guid userId, CancellationToken ct = default) =>
        await _context.WishlistItems
            .Include(w => w.Product).ThenInclude(p => p.Images)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken ct = default) =>
        await _context.WishlistItems.AnyAsync(w => w.UserId == userId && w.ProductId == productId, ct);

    public async Task AddAsync(WishlistItem item, CancellationToken ct = default)
    {
        await _context.WishlistItems.AddAsync(item, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, ct);

        if (item is not null)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync(ct);
        }
    }
}
