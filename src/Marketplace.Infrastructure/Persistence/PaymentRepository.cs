using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default) =>
        await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);

    public async Task<Payment?> GetByProviderPaymentIdAsync(string providerPaymentId, CancellationToken ct = default) =>
        await _context.Payments.FirstOrDefaultAsync(p => p.ProviderPaymentId == providerPaymentId, ct);

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        await _context.Payments.AddAsync(payment, ct);
        await _context.SaveChangesAsync(ct);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
        _context.SaveChanges();
    }
}
