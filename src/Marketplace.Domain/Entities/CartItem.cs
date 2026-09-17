namespace Marketplace.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; } // السعر وقت الإضافة، مش السعر الحالي بالضرورة
}
