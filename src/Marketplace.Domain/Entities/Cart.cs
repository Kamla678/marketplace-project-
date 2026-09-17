using Marketplace.Domain.Exceptions;

namespace Marketplace.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public decimal Subtotal => Items.Sum(i => i.UnitPriceSnapshot * i.Quantity);

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            existing.UnitPriceSnapshot = unitPrice; // نحدث السعر لآخر سعر معروف وقت الإضافة
        }
        else
        {
            Items.Add(new CartItem
            {
                CartId = Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPriceSnapshot = unitPrice
            });
        }
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new DomainException("Item not found in cart.");

        if (quantity <= 0)
        {
            Items.Remove(item);
            return;
        }

        item.Quantity = quantity;
    }

    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
            Items.Remove(item);
    }

    public void Clear() => Items.Clear();
}
