using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is not null)
            return cart;

        cart = new Cart { UserId = userId };
        await _context.Carts.AddAsync(cart, ct);
        await _context.SaveChangesAsync(ct); // لازم نحفظ فورًا عشان الـCart.Id يبقى صالح لأي CartItem يتضاف بعده

        return cart;
    }

    public void Update(Cart cart) => _context.Carts.Update(cart);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
}
