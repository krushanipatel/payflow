namespace PayFlow.Domain.Payments;

public sealed class LedgerEntry
{
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "";
    public EntryDirection Direction { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private LedgerEntry() { } // for EF Core later

    internal LedgerEntry(Guid paymentId, Guid accountId, decimal amount, string currency, EntryDirection direction)
    {
        Id = Guid.NewGuid();
        PaymentId = paymentId;
        AccountId = accountId;
        Amount = amount;
        Currency = currency;
        Direction = direction;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}