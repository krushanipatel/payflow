using PayFlow.Domain.Payments;

namespace PayFlow.Application.Payments;

public sealed record CreatePaymentCommand(
    string IdempotencyKey, Guid FromAccountId, Guid ToAccountId, decimal Amount, string Currency);

public sealed record CreatePaymentResult(Payment Payment, bool IsDuplicate);

public sealed class CreatePaymentService(IPaymentRepository repository)
{
    public async Task<CreatePaymentResult> HandleAsync(CreatePaymentCommand command, CancellationToken ct = default)
    {
        // 1. Seen this key before? Return the payment we already have. Don't charge twice.
        var existing = await repository.GetByIdempotencyKeyAsync(command.IdempotencyKey, ct);
        if (existing is not null)
            return new CreatePaymentResult(existing, IsDuplicate: true);

        // 2. New key: the Domain checks the rules and writes the 2 ledger lines.
        var payment = Payment.Create(
            command.IdempotencyKey, command.FromAccountId, command.ToAccountId, command.Amount, command.Currency);
        var entries = payment.Complete();

        // 3. Save. If another request with the same key saved a split second before us, return that one.
        if (!await repository.TryAddAsync(payment, entries, ct))
        {
            var winner = await repository.GetByIdempotencyKeyAsync(command.IdempotencyKey, ct);
            return new CreatePaymentResult(winner!, IsDuplicate: true);
        }

        return new CreatePaymentResult(payment, IsDuplicate: false);
    }
}