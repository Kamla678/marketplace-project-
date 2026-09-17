using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Entities;

public class SellerProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string StoreName { get; set; } = default!;
    public string? StoreDescription { get; set; }
    public string? StoreLogoUrl { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public decimal CommissionRate { get; set; } = 10.0m;
}
