using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // لضمان إن العميل يشوف بس طلباته هو
    Task<Order?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<PagedResult<Order>> GetForUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);

    // كل الـOrderItems اللي فيها منتجات الـSeller ده، من أي Order
    Task<PagedResult<OrderItem>> GetItemsForSellerAsync(Guid sellerId, int page, int pageSize, CancellationToken ct = default);

    Task AddAsync(Order order, CancellationToken ct = default);
    void Update(Order order);
}
