using WeMoney.Infrastructure.Security;

namespace WeMoney.IntegrationTests;

public class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_IsNotPlainTextAndIsSalted()
    {
        var first = _hasher.Hash("12345678");
        var second = _hasher.Hash("12345678");

        Assert.DoesNotContain("12345678", first);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_AcceptsCorrectAndRejectsWrongPassword()
    {
        var hash = _hasher.Hash("12345678");

        Assert.True(_hasher.Verify("12345678", hash));
        Assert.False(_hasher.Verify("87654321", hash));
        Assert.False(_hasher.Verify("12345678", "formato-invalido"));
    }
}
