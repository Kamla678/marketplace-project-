using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface ICouponRepository
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<List<Coupon>> GetForSellerAsync(Guid sellerId, CancellationToken ct = default); // بيرجع كوبونات الـSeller ده بس (SellerId=null مش هيرجع هنا)
    Task AddAsync(Coupon coupon, CancellationToken ct = default);
    void Update(Coupon coupon);
}
