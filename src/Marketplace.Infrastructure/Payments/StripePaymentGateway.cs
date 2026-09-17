using Marketplace.Application.Common.Interfaces;
using Stripe;

namespace Marketplace.Infrastructure.Payments;

public class StripeSettings
{
    // مفاتيح Test Mode بتبدأ بـ sk_test_ / whsec_test_...
    // لازم تيجي من appsettings.Development.json أو Environment Variable، مش هارد كودد
    public string SecretKey { get; set; } = default!;
    public string WebhookSecret { get; set; } = default!;
}

public class StripePaymentGateway : IPaymentGateway
{
    public string ProviderName => "Stripe";

    public StripePaymentGateway(Microsoft.Extensions.Options.IOptions<StripeSettings> options)
    {
        // StripeConfiguration.ApiKey static — ده الأسلوب القياسي في مكتبة Stripe.net
        StripeConfiguration.ApiKey = options.Value.SecretKey;
        _webhookSecret = options.Value.WebhookSecret;
    }

    private readonly string _webhookSecret;

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(Guid orderId, decimal amount, string currency, CancellationToken ct = default)
    {
        var service = new PaymentIntentService();

        var options = new PaymentIntentCreateOptions
        {
            // Stripe بياخد المبلغ بأصغر وحدة عملة (سنت) — عشان كده *100
            Amount = (long)(amount * 100),
            Currency = currency,
            Metadata = new Dictionary<string, string> { { "orderId", orderId.ToString() } },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
        };

        var intent = await service.CreateAsync(options, cancellationToken: ct);

        return new PaymentIntentResult(intent.Id, intent.ClientSecret);
    }

    public Task<WebhookPaymentEvent> ParseWebhookAsync(string requestBody, string? signatureHeader, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(signatureHeader))
            throw new StripeException("Missing Stripe-Signature header.");

        // EventUtility.ConstructEvent بيتحقق من الـsignature باستخدام الـWebhookSecret —
        // لو حد بعت request مزيف، ده هيرمي StripeException هنا ومايكملش خالص
        var stripeEvent = EventUtility.ConstructEvent(requestBody, signatureHeader, _webhookSecret);

        return Task.FromResult(stripeEvent.Type switch
        {
            "payment_intent.succeeded" => new WebhookPaymentEvent(
                ((PaymentIntent)stripeEvent.Data.Object).Id, IsSuccess: true, FailureReason: null),

            "payment_intent.payment_failed" => new WebhookPaymentEvent(
                ((PaymentIntent)stripeEvent.Data.Object).Id, IsSuccess: false,
                FailureReason: ((PaymentIntent)stripeEvent.Data.Object).LastPaymentError?.Message),

            _ => throw new NotSupportedException($"Unhandled Stripe event type: {stripeEvent.Type}")
        });
    }
}
