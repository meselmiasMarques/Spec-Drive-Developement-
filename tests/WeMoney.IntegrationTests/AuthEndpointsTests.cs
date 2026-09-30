using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WeMoney.IntegrationTests;

public class AuthEndpointsTests(WeMoneyApiFactory factory) : IClassFixture<WeMoneyApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    // REQ-001 CA-001
    [Fact]
    public async Task Register_WithValidData_Returns201WithoutPassword()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { name = "João", email = "joao.register@email.com", password = "12345678" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("joao.register@email.com", body);
        Assert.DoesNotContain("12345678", body);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
    }

    // REQ-001 CA-002
    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var payload = new { name = "João", email = "dup@email.com", password = "12345678" };
        await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // REQ-001 CA-003
    [Fact]
    public async Task Register_WithShortPassword_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { name = "João", email = "short@email.com", password = "1234567" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("", "valid@email.com")]
    [InlineData("João", "invalido")]
    public async Task Register_WithInvalidData_Returns400(string name, string email)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", new { name, email, password = "12345678" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // REQ-002 CA-001
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { name = "João", email = "login@email.com", password = "12345678" });

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "login@email.com", password = "12345678" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(body.GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { name = "João", email = "wrong@email.com", password = "12345678" });

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "wrong@email.com", password = "senhaerrada" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/categories")]
    [InlineData("/api/v1/transactions")]
    [InlineData("/api/v1/transactions/balance")]
    public async Task ProtectedEndpoints_WithoutToken_Return401(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithTamperedToken_Returns401()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxIn0.invalid");

        var response = await client.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
