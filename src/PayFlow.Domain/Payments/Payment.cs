namespace PayFlow.Domain.Payments;

public sealed class Payment
{
    public Guid Id { get; private set; }
    public string IdempotencyKey { get; private set; } = "";
    public Guid FromAccountId { get; private set; }
    public Guid ToAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "";
    public PaymentStatus Status { get; private set; }
    public string? HoldReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Payment() { } // for EF Core later

    public static Payment Create(string idempotencyKey, Guid fromAccountId, Guid toAccountId, decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("Idempotency key is required.", nameof(idempotencyKey));
        if (fromAccountId == toAccountId)
            throw new ArgumentException("Cannot pay the same account.", nameof(toAccountId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be more than zero.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter code like CAD.", nameof(currency));

        return new Payment
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    // Completes the payment and returns the two ledger lines: debit sender, credit receiver.
    public IReadOnlyList<LedgerEntry> Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Only a pending payment can be completed. Status is {Status}.");

        Status = PaymentStatus.Completed;
        return
        [
            new LedgerEntry(Id, FromAccountId, Amount, Currency, EntryDirection.Debit),
            new LedgerEntry(Id, ToAccountId, Amount, Currency, EntryDirection.Credit)
        ];
    }

    public void Hold(string reason)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Only a pending payment can be held. Status is {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A held payment needs a reason Priya can read.", nameof(reason));

        Status = PaymentStatus.Held;
        HoldReason = reason;
    }
}