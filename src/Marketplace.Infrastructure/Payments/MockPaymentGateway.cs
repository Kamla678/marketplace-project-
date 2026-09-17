using Marketplace.Application.Common.Interfaces;

namespace Marketplace.Infrastructure.Payments;

// Gateway وهمي بالكامل — مفيد لو مش عايزة تعملي حساب Stripe أصلًا،
// أو عايزة تجربي/تعرضي المشروع من غير اتصال إنترنت خالص.
// بيحاكي نجاح الدفع فورًا (أو ممكن تعدليه يفشل عشوائيًا لو عايزة تختبري الـfailure path).
public class MockPaymentGateway : IPaymentGateway
{
    public string ProviderName => "Mock";

    public Task<PaymentIntentResult> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, CancellationToken ct = default)
    {
        var fakeId = $"mock_pi_{Guid.NewGuid():N}";
        var fakeClientSecret = $"mock_secret_{Guid.NewGuid():N}";

        return Task.FromResult(new PaymentIntentResult(fakeId, fakeClientSecret));
    }

    public Task<WebhookPaymentEvent> ParseWebhookAsync(string requestBody, string? signatureHeader, CancellationToken ct = default)
    {
        // مفيش signature حقيقي نتحقق منه هنا لأن مفيش Provider خارجي أصلًا —
        // في الـMock mode، استخدمي endpoint الـ"simulate" بدل الـwebhook الحقيقي (راجعي PaymentsController)
        throw new NotSupportedException(
            "MockPaymentGateway doesn't receive real webhooks. Use POST /payments/{orderId}/simulate instead for local testing.");
    }
}
