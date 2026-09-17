using Marketplace.Domain.Enums;
using Marketplace.Domain.Exceptions;

namespace Marketplace.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = default!;
    public Guid UserId { get; set; }
    public Guid? CouponId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }

    // Shipping address snapshot — بتتاخد نسخة وقت الـcheckout، مش reference حي لعنوان ممكن يتغير بعدين
    public string ShippingFullName { get; set; } = default!;
    public string ShippingPhone { get; set; } = default!;
    public string ShippingAddressLine { get; set; } = default!;
    public string ShippingCity { get; set; } = default!;
    public string ShippingCountry { get; set; } = default!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed.");
        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new DomainException("Cannot cancel an order that has already been shipped or delivered.");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
