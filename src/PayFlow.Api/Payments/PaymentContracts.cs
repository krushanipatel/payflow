using PayFlow.Domain.Payments;

namespace PayFlow.Api.Payments;

// What the caller sends in the JSON body.
public sealed record CreatePaymentRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount, string Currency);

// What we send back. We never return the Domain object directly.
public sealed record PaymentResponse(
    Guid Id, Guid FromAccountId, Guid ToAccountId, decimal Amount, string Currency, string Status, DateTimeOffset CreatedAt)
{
    public static PaymentResponse From(Payment p) =>
        new(p.Id, p.FromAccountId, p.ToAccountId, p.Amount, p.Currency, p.Status.ToString(), p.CreatedAt);
}