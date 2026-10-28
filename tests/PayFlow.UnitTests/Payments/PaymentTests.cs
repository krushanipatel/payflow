using PayFlow.Domain.Payments;

namespace PayFlow.UnitTests.Payments;

public class PaymentTests
{
    private static readonly Guid Alice = Guid.NewGuid();
    private static readonly Guid Bob = Guid.NewGuid();

    private static Payment NewPayment(decimal amount = 100m) =>
        Payment.Create("key-123", Alice, Bob, amount, "cad");

    [Fact]
    public void Create_ValidPayment_IsPendingWithKey()
    {
        var payment = NewPayment();

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal("key-123", payment.IdempotencyKey);
        Assert.Equal("CAD", payment.Currency);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ZeroOrNegativeAmount_Throws(decimal amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewPayment(amount));
    }

    [Fact]
    public void Create_WithoutIdempotencyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => Payment.Create("", Alice, Bob, 10m, "CAD"));
    }

    [Fact]
    public void Create_SameFromAndTo_Throws()
    {
        Assert.Throws<ArgumentException>(() => Payment.Create("k", Alice, Alice, 10m, "CAD"));
    }

    [Fact]
    public void Complete_WritesOneDebitAndOneCreditForSameAmount()
    {
        var payment = NewPayment(250m);

        var entries = payment.Complete();

        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.Equal(2, entries.Count);
        var debit = Assert.Single(entries, e => e.Direction == EntryDirection.Debit);
        var credit = Assert.Single(entries, e => e.Direction == EntryDirection.Credit);
        Assert.Equal(Alice, debit.AccountId);
        Assert.Equal(Bob, credit.AccountId);
        Assert.Equal(debit.Amount, credit.Amount);
    }

    [Fact]
    public void Complete_Twice_Throws()
    {
        var payment = NewPayment();
        payment.Complete();

        Assert.Throws<InvalidOperationException>(() => payment.Complete());
    }

    [Fact]
    public void Hold_SetsStatusAndReason()
    {
        var payment = NewPayment();

        payment.Hold("Amount is over the $10,000 daily limit.");

        Assert.Equal(PaymentStatus.Held, payment.Status);
        Assert.Equal("Amount is over the $10,000 daily limit.", payment.HoldReason);
    }
}