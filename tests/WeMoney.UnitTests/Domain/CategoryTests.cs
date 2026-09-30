using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.UnitTests.Domain;

public class CategoryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")]
    public void Constructor_WithNameShorterThan3_Throws(string? name)
    {
        Assert.Throws<DomainException>(() => new Category(Guid.NewGuid(), name!, TransactionType.EXPENSE));
    }

    [Fact]
    public void Constructor_WithNameLongerThan100_Throws()
    {
        Assert.Throws<DomainException>(() => new Category(Guid.NewGuid(), new string('a', 101), TransactionType.EXPENSE));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(100)]
    public void Constructor_WithNameAtLimits_Succeeds(int length)
    {
        var category = new Category(Guid.NewGuid(), new string('a', length), TransactionType.INCOME);
        Assert.Equal(length, category.Name.Length);
    }

    [Fact]
    public void Constructor_WithInvalidType_Throws()
    {
        Assert.Throws<DomainException>(() => new Category(Guid.NewGuid(), "Casa", (TransactionType)99));
    }

    [Fact]
    public void Constructor_WithoutUser_Throws()
    {
        Assert.Throws<DomainException>(() => new Category(Guid.Empty, "Casa", TransactionType.EXPENSE));
    }
}
