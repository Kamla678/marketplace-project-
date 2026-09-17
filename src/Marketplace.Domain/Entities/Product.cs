using Marketplace.Domain.Enums;
using Marketplace.Domain.Exceptions;

namespace Marketplace.Domain.Entities;

public class Product : BaseEntity
{
    public Guid SellerId { get; set; }
    public SellerProfile Seller { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Description { get; set; } = default!;

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = default!;

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? RejectionReason { get; set; }
    public bool IsActive { get; set; } = true;

    public decimal AvgRating { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public int ViewsCount { get; set; } = 0;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // ---- Domain behavior (مش مجرد getters/setters) ----

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        if (StockQuantity < quantity)
            throw new DomainException($"Insufficient stock. Available: {StockQuantity}, requested: {quantity}.");

        StockQuantity -= quantity;

        // لو خلص المخزون، المنتج يختفي تلقائيًا من واجهة العميل
        if (StockQuantity == 0)
            IsActive = false;
    }

    public void RestoreStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice, decimal? discountPrice = null)
    {
        if (newPrice <= 0)
            throw new DomainException("Price must be greater than zero.");

        if (discountPrice is not null && discountPrice >= newPrice)
            throw new DomainException("Discount price must be less than the original price.");

        Price = newPrice;
        DiscountPrice = discountPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        ApprovalStatus = ApprovalStatus.Approved;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        ApprovalStatus = ApprovalStatus.Rejected;
        RejectionReason = reason;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
