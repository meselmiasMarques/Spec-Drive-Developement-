using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_NormalizesEmail()
    {
        var user = new User(" João ", " Joao@Email.com ", "hash");

        Assert.Equal("João", user.Name);
        Assert.Equal("joao@email.com", user.Email);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithoutName_Throws(string? name)
    {
        Assert.Throws<DomainException>(() => new User(name!, "a@b.com", "hash"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalido")]
    [InlineData("a@b")]
    [InlineData("a b@c.com")]
    public void NormalizeEmail_WithInvalidEmail_Throws(string? email)
    {
        Assert.Throws<DomainException>(() => User.NormalizeEmail(email));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567")]
    public void ValidatePassword_WithLessThan8Chars_Throws(string? password)
    {
        Assert.Throws<DomainException>(() => User.ValidatePassword(password));
    }

    [Fact]
    public void ValidatePassword_With8Chars_DoesNotThrow()
    {
        User.ValidatePassword("12345678");
    }
}
