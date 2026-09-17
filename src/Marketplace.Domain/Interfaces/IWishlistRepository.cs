using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface IWishlistRepository
{
    Task<List<WishlistItem>> GetForUserAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task AddAsync(WishlistItem item, CancellationToken ct = default);
    Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default);
}
