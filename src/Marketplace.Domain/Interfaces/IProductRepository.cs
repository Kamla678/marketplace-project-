using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Interfaces;

public record ProductFilter(
    Guid? CategoryId = null,
    Guid? SellerId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    bool OnlyApprovedAndActive = true,
    ApprovalStatus? ApprovalStatus = null, // لو محددة، بتلغي تأثير OnlyApprovedAndActive وتفلتر بالـstatus ده بالظبط
    int Page = 1,
    int PageSize = 20
);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // بيرجع المنتج بس لو مملوك للـSellerId المحدد — استخدام مباشر في الـauthorization check
    Task<Product?> GetByIdForSellerAsync(Guid id, Guid sellerId, CancellationToken ct = default);

    Task<PagedResult<Product>> GetPagedAsync(ProductFilter filter, CancellationToken ct = default);

    Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default);

    Task AddAsync(Product product, CancellationToken ct = default);
    void Update(Product product);
}
