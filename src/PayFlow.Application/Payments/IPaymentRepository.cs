using PayFlow.Domain.Payments;

namespace PayFlow.Application.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default);

    // Returns false if a payment with the same idempotency key is already saved.
    Task<bool> TryAddAsync(Payment payment, IReadOnlyList<LedgerEntry> entries, CancellationToken ct = default);
}