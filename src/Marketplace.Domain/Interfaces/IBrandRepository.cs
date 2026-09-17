using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface IBrandRepository
{
    Task<List<Brand>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Brand brand, CancellationToken ct = default);
}
