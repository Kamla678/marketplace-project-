using Marketplace.Domain.Exceptions;

namespace Marketplace.Domain.Entities;

public class Review : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = true; // ممكن تتغير لـfalse لو حبينا moderation queue لاحقًا

    // نتيجة AI Review Analyzer (Phase 6) — nullable لحد ما الـAI يعالجها
    public string? SentimentLabel { get; set; }

    public static Review Create(Guid productId, Guid userId, int rating, string? comment)
    {
        if (rating is < 1 or > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        return new Review { ProductId = productId, UserId = userId, Rating = rating, Comment = comment };
    }
}
