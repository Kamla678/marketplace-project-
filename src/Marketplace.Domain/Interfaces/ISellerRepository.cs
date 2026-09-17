using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface ISellerRepository
{
    Task<SellerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
