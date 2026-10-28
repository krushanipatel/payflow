using System.Collections.Concurrent;
using PayFlow.Application.Payments;
using PayFlow.Domain.Payments;

namespace PayFlow.Infrastructure.Payments;

// Keeps payments in memory while the app runs. Day 4 replaces this with SQL Server (EF Core).
public sealed class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<string, Payment> _paymentsByKey = new();
    private readonly ConcurrentDictionary<Guid, IReadOnlyList<LedgerEntry>> _ledgerByPaymentId = new();

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult<Payment?>(_paymentsByKey.Values.FirstOrDefault(p => p.Id == id));

    public Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default) =>
        Task.FromResult<Payment?>(_paymentsByKey.TryGetValue(idempotencyKey, out var payment) ? payment : null);

    public Task<bool> TryAddAsync(Payment payment, IReadOnlyList<LedgerEntry> entries, CancellationToken ct = default)
    {
        // TryAdd is atomic: if two requests use the same key at once, only one gets in.
        if (!_paymentsByKey.TryAdd(payment.IdempotencyKey, payment))
            return Task.FromResult(false);

        _ledgerByPaymentId[payment.Id] = entries;
        return Task.FromResult(true);
    }
}