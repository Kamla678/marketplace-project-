using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = default!;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public Guid? SellerId { get; set; } // null = كوبون عام على مستوى المنصة
    public DateTime ExpiryDate { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }

    public bool IsValidFor(decimal subtotal)
    {
        if (ExpiryDate < DateTime.UtcNow) return false;
        if (UsedCount >= UsageLimit) return false;
        if (MinOrderAmount.HasValue && subtotal < MinOrderAmount.Value) return false;
        return true;
    }

    public decimal CalculateDiscount(decimal subtotal)
    {
        var discount = DiscountType == DiscountType.Percentage
            ? subtotal * (DiscountValue / 100m)
            : DiscountValue;

        // الخصم مايتجاوزش الـSubtotal نفسه (منطقي، مايبقاش سالب)
        return Math.Min(discount, subtotal);
    }

    public void RecordUsage() => UsedCount++;
}
