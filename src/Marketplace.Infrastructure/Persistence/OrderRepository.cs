using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<Order?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId, ct);

    public async Task<PagedResult<Order>> GetForUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<Order>(items, page, pageSize, totalCount);
    }

    public async Task<PagedResult<OrderItem>> GetItemsForSellerAsync(Guid sellerId, int page, int pageSize, CancellationToken ct = default)
    {
        // كل الـOrderItems بتاعة الـSeller ده، من أي Order — ده اللي بيغذي "Seller Orders Dashboard"
        var query = _context.OrderItems
            .Include(i => i.Order)
            .Include(i => i.Product)
            .Where(i => i.SellerId == sellerId)
            .OrderByDescending(i => i.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<OrderItem>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
    }
}
