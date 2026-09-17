using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Brands.OrderBy(b => b.Name).ToListAsync(ct);

    public async Task AddAsync(Brand brand, CancellationToken ct = default)
    {
        await _context.Brands.AddAsync(brand, ct);
        await _context.SaveChangesAsync(ct);
    }
}
