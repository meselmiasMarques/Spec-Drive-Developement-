using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeMoney.IntegrationTests;

public static class ApiClient
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(WeMoneyApiFactory factory, string? email = null)
    {
        var client = factory.CreateClient();
        email ??= $"{Guid.NewGuid():N}@email.com";

        var register = await client.PostAsJsonAsync("/api/v1/auth/register", new { name = "Usuário", email, password = "12345678" });
        register.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "12345678" });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<JsonElement>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body.GetProperty("accessToken").GetString());
        return client;
    }

    public static async Task<Guid> CreateCategoryAsync(HttpClient client, string name = "Mercado", string type = "EXPENSE")
    {
        var response = await client.PostAsJsonAsync("/api/v1/categories", new { name, type });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    public static async Task<Guid> CreateTransactionAsync(HttpClient client, Guid categoryId, decimal amount = 250.50m, string type = "EXPENSE")
    {
        var response = await client.PostAsJsonAsync("/api/v1/transactions",
            new { description = "Supermercado", amount, type, categoryId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }
}
