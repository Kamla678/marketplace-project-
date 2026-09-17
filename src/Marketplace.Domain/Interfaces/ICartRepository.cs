using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface ICartRepository
{
    // بيرجع كارت المستخدم، وينشئ واحد جديد لو مش موجود (كل مستخدم له كارت واحد بس)
    Task<Cart> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default);
    void Update(Cart cart);
    Task SaveChangesAsync(CancellationToken ct = default);
}
