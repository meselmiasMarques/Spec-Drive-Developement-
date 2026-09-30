using WeMoney.Web.Services;

namespace WeMoney.UnitTests.Web;

public class ValidatorsTests
{
    [Theory]
    [InlineData("joao@email.com")]
    [InlineData(" joao@email.com ")]
    [InlineData("")]
    [InlineData(null)]
    public void Email_ValidOrEmpty_ReturnsNull(string? email)
    {
        Assert.Null(Validators.Email(email));
    }

    [Theory]
    [InlineData("joao")]
    [InlineData("joao@")]
    [InlineData("João <joao@email.com>")]
    public void Email_Invalid_ReturnsError(string email)
    {
        Assert.NotNull(Validators.Email(email));
    }

    [Fact]
    public void Password_ShorterThan8_ReturnsError()
    {
        Assert.NotNull(Validators.Password("1234567"));
        Assert.Null(Validators.Password("12345678"));
    }

    [Theory]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("  ab  ", false)]
    public void CategoryName_ChecksMinimumLength(string name, bool valid)
    {
        Assert.Equal(valid, Validators.CategoryName(name) is null);
    }

    [Fact]
    public void CategoryName_LongerThan100_ReturnsError()
    {
        Assert.NotNull(Validators.CategoryName(new string('a', 101)));
        Assert.Null(Validators.CategoryName(new string('a', 100)));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(0.01, true)]
    public void Amount_MustBeGreaterThanZero(decimal amount, bool valid)
    {
        Assert.Equal(valid, Validators.Amount(amount) is null);
    }
}
