using Marketplace.Domain.Enums;
using Marketplace.Domain.Exceptions;

namespace Marketplace.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string Provider { get; set; } = default!; // "Stripe" أو "Mock"
    public string? ProviderPaymentId { get; set; } // client_secret/payment_intent id من Stripe
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }

    public void MarkAsPaid()
    {
        if (Status == PaymentStatus.Paid)
            return; // idempotent — الـwebhook ممكن يوصل مرتين، مانعملش error ولا نكرر أي منطق

        if (Status == PaymentStatus.Refunded)
            throw new DomainException("Cannot mark a refunded payment as paid.");

        Status = PaymentStatus.Paid;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
