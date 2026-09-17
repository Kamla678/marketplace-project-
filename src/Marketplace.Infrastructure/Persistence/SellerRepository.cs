using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class SellerRepository : ISellerRepository
{
    private readonly ApplicationDbContext _context;

    public SellerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SellerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.SellerProfiles.FirstOrDefaultAsync(s => s.Id == id, ct);
}
