using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.UnitTests.Domain;

public class TransactionTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Category Category = new(UserId, "Mercado", TransactionType.EXPENSE);
    private static readonly DateOnly Today = new(2026, 9, 25);

    [Fact]
    public void Constructor_WithValidData_Succeeds()
    {
        var transaction = new Transaction(UserId, Category, "Supermercado", 250.50m, TransactionType.EXPENSE, Today);

        Assert.Equal(Category.Id, transaction.CategoryId);
        Assert.Equal(250.50m, transaction.Amount);
        Assert.Equal(Today, transaction.TransactionDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithAmountNotGreaterThanZero_Throws(decimal amount)
    {
        Assert.Throws<DomainException>(() =>
            new Transaction(UserId, Category, "Supermercado", amount, TransactionType.EXPENSE, Today));
    }

    [Fact]
    public void Constructor_WithCategoryFromAnotherUser_Throws()
    {
        var otherCategory = new Category(Guid.NewGuid(), "Outro", TransactionType.EXPENSE);

        Assert.Throws<DomainException>(() =>
            new Transaction(UserId, otherCategory, "Supermercado", 10m, TransactionType.EXPENSE, Today));
    }

    [Fact]
    public void Constructor_WithInvalidType_Throws()
    {
        Assert.Throws<DomainException>(() =>
            new Transaction(UserId, Category, "Supermercado", 10m, (TransactionType)99, Today));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_WithoutDescription_Throws(string description)
    {
        Assert.Throws<DomainException>(() =>
            new Transaction(UserId, Category, description, 10m, TransactionType.EXPENSE, Today));
    }
}
