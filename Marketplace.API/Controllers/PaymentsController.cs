using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Payments.Commands.CreatePaymentIntent;
using Marketplace.Application.Features.Payments.Commands.HandleWebhook;
using Marketplace.Application.Features.Payments.Commands.SimulatePayment;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;

    public PaymentsController(IMediator mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpPost("create-intent")]
    [Authorize]
    [ProducesResponseType(typeof(CreatePaymentIntentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIntent(CreatePaymentIntentRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreatePaymentIntentCommand(request.OrderId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Webhook حقيقي من Stripe. لازم يتقرا كـraw body، مش JSON model-binding عادي،
    /// عشان الـsignature verification محتاج البايتات الأصلية بالظبط زي ما بعتها Stripe.
    /// في Test Mode: استخدمي Stripe CLI محليًا (`stripe listen --forward-to localhost:{port}/api/v1/payments/webhook`).
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous] // Stripe نفسه هو اللي بيبعت هنا، مش مستخدم مسجل دخوله — الأمان بييجي من الـsignature مش من الـAuth
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        // لو الـsignature غلط، الـHandler بيرمي exception وده بيترجم لـ400 عن طريق الـExceptionHandlingMiddleware —
        // احنا متعمدين مانمسكهاش هنا عشان نضمن إن أي فشل في التحقق يوقف العملية فورًا
        await _mediator.Send(new HandleWebhookCommand(body, signature), ct);

        return Ok();
    }

    /// <summary>
    /// Dev-only: محاكاة نجاح/فشل الدفع محليًا من غير الحاجة لـwebhook حقيقي من Stripe (زي لما تستخدمي MockPaymentGateway،
    /// أو لو مش عايزة تشغلي Stripe CLI). بيترفض في Production تلقائيًا.
    /// </summary>
    [HttpPost("{orderId:guid}/simulate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Simulate(Guid orderId, [FromQuery] bool success = true, CancellationToken ct = default)
    {
        if (!_environment.IsDevelopment())
            return Forbid(); // حماية إضافية — الـendpoint ده خطير لو اشتغل في production فعليًا

        await _mediator.Send(new SimulatePaymentCommand(orderId, success), ct);
        return NoContent();
    }
}

public record CreatePaymentIntentRequest(Guid OrderId);
