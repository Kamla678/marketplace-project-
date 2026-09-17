using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    // الاستعلام ده بيفلتر بـSellerId جوه الـWHERE نفسه —
    // يعني حتى لو الـid صح بس SellerId مختلف، هيرجع null مش exception ولا بيانات حد تاني
    public async Task<Product?> GetByIdForSellerAsync(Guid id, Guid sellerId, CancellationToken ct = default) =>
        await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId, ct);

    public async Task<PagedResult<Product>> GetPagedAsync(ProductFilter filter, CancellationToken ct = default)
    {
        var query = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .AsQueryable();

        if (filter.ApprovalStatus.HasValue)
        {
            // فلترة صريحة بحالة معينة (مثلاً Pending للأدمن) — بتلغي شرط OnlyApprovedAndActive
            query = query.Where(p => p.ApprovalStatus == filter.ApprovalStatus.Value);
        }
        else if (filter.OnlyApprovedAndActive)
        {
            query = query.Where(p => p.ApprovalStatus == ApprovalStatus.Approved && p.IsActive);
        }

        if (filter.SellerId.HasValue)
            query = query.Where(p => p.SellerId == filter.SellerId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            // ملاحظة: ده SQL LIKE بسيط لأغراض الـskeleton —
            // في Phase 5 ده هيتستبدل بـElasticsearch query حقيقي (راجعي phase5-search-performance.md)
            var term = filter.SearchTerm.Trim();
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{term}%"));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>(items, filter.Page, filter.PageSize, totalCount);
    }

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default) =>
        await _context.Products.AnyAsync(p => p.SKU == sku, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }
}
