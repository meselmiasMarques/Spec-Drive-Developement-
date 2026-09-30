using System.Text;
using System.Text.Json;
using WeMoney.Web.Services;

namespace WeMoney.UnitTests.Web;

public class JwtParserTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 26, 12, 0, 0, TimeSpan.Zero);

    private static string Token(object payload)
    {
        static string Encode(string json) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{Encode("{\"alg\":\"HS256\"}")}.{Encode(JsonSerializer.Serialize(payload))}.assinatura";
    }

    [Fact]
    public void Parse_ValidToken_ReturnsAuthenticatedUserWithClaims()
    {
        var token = Token(new { sub = "user-id", name = "João", email = "joao@email.com", exp = Now.AddMinutes(30).ToUnixTimeSeconds() });

        var user = JwtParser.Parse(token, Now);

        Assert.NotNull(user);
        Assert.True(user.Identity!.IsAuthenticated);
        Assert.Equal("João", user.Identity.Name);
        Assert.Equal("user-id", user.FindFirst("sub")!.Value);
        Assert.Equal("joao@email.com", user.FindFirst("email")!.Value);
    }

    [Fact]
    public void Parse_ExpiredToken_ReturnsNull()
    {
        var token = Token(new { sub = "user-id", exp = Now.AddSeconds(-1).ToUnixTimeSeconds() });

        Assert.Null(JwtParser.Parse(token, Now));
    }

    [Fact]
    public void Parse_TokenWithoutExp_ReturnsNull()
    {
        Assert.Null(JwtParser.Parse(Token(new { sub = "user-id" }), Now));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nao-e-um-jwt")]
    [InlineData("a.@@@.c")]
    [InlineData("a.bm90LWpzb24.c")]
    public void Parse_MalformedToken_ReturnsNull(string? token)
    {
        Assert.Null(JwtParser.Parse(token, Now));
    }
}
