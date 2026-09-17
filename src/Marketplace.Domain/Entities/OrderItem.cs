using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    // مكرر عمدًا من الـProduct عشان كل Seller يقدر يفلتر طلباته من غير Join إضافي،
    // وعشان لو الـProduct اتنقل ملكيته يومًا (نادر) الطلبات القديمة تفضل صحيحة تاريخيًا
    public Guid SellerId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }

    // حالة مستقلة لكل Seller — Seller A يقدر يشحن بضاعته وSeller B لسه بيجهز
    public OrderStatus ItemStatus { get; set; } = OrderStatus.Pending;
}
