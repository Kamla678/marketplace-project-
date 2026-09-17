using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _context;

    public CouponRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), ct);

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default) =>
        await _context.Coupons.AnyAsync(c => c.Code == code.ToUpperInvariant(), ct);

    public async Task<List<Coupon>> GetForSellerAsync(Guid sellerId, CancellationToken ct = default) =>
        await _context.Coupons
            .Where(c => c.SellerId == sellerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Coupon coupon, CancellationToken ct = default)
    {
        await _context.Coupons.AddAsync(coupon, ct);
        await _context.SaveChangesAsync(ct);
    }

    public void Update(Coupon coupon) => _context.Coupons.Update(coupon);
}
