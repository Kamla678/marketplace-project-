using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Phone { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;

    // Refresh token — بسيط لأغراض Phase 1، ممكن ينقل لجدول منفصل لو حبينا Multi-device support
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    // 1-1 مع Seller profile، nullable لأن مش كل User هيبقى Seller
    public SellerProfile? SellerProfile { get; set; }

    // ملاحظة: الـDomain متعمدش يعرف حاجة عن BCrypt أو أي مكتبة تشفير —
    // التحقق الفعلي من الـhash بيتم في Infrastructure (IPasswordHasher)
    // عشان الـDomain يفضل نضيف بدون dependencies خارجية (Clean Architecture rule).
    public bool IsRefreshTokenExpired => RefreshTokenExpiresAt is null || RefreshTokenExpiresAt < DateTime.UtcNow;
}
