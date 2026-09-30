using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WeMoney.IntegrationTests;

public class TransactionEndpointsTests(WeMoneyApiFactory factory) : IClassFixture<WeMoneyApiFactory>
{
    [Fact]
    public async Task Crud_Works_ForOwner()
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var categoryId = await ApiClient.CreateCategoryAsync(client);

        var create = await client.PostAsJsonAsync("/api/v1/transactions",
            new { description = "Supermercado", amount = 250.50m, type = "EXPENSE", categoryId, transactionDate = "2026-09-01" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid();
        Assert.Equal("2026-09-01", created.GetProperty("transactionDate").GetString());

        var update = await client.PutAsJsonAsync($"/api/v1/transactions/{id}",
            new { description = "Feira", amount = 80m, type = "EXPENSE", categoryId });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var list = await client.GetFromJsonAsync<JsonElement>("/api/v1/transactions");
        Assert.Equal("Feira", list[0].GetProperty("description").GetString());

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/v1/transactions/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/transactions/{id}")).StatusCode);
    }

    // RN-001 / RN-002 / RN-005
    [Theory]
    [InlineData(0, "EXPENSE", true)]
    [InlineData(-5, "EXPENSE", true)]
    [InlineData(10, "OUTRO", true)]
    [InlineData(10, "EXPENSE", false)]
    public async Task Create_WithInvalidData_Returns400(decimal amount, string type, bool validCategory)
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var categoryId = validCategory ? await ApiClient.CreateCategoryAsync(client) : Guid.NewGuid();

        var response = await client.PostAsJsonAsync("/api/v1/transactions",
            new { description = "Teste", amount, type, categoryId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // RN-003
    [Fact]
    public async Task Create_WithCategoryFromAnotherUser_Returns400()
    {
        var owner = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var intruder = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var ownerCategory = await ApiClient.CreateCategoryAsync(owner);

        var response = await intruder.PostAsJsonAsync("/api/v1/transactions",
            new { description = "Teste", amount = 10m, type = "EXPENSE", categoryId = ownerCategory });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // RN-004 / SPEC-006 — isolamento de dados
    [Fact]
    public async Task OtherUser_CannotSeeOrChangeTransaction()
    {
        var owner = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var intruder = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var ownerCategory = await ApiClient.CreateCategoryAsync(owner);
        var intruderCategory = await ApiClient.CreateCategoryAsync(intruder);
        var id = await ApiClient.CreateTransactionAsync(owner, ownerCategory);

        Assert.Equal(HttpStatusCode.NotFound, (await intruder.GetAsync($"/api/v1/transactions/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await intruder.PutAsJsonAsync($"/api/v1/transactions/{id}",
            new { description = "Hack", amount = 1m, type = "EXPENSE", categoryId = intruderCategory })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await intruder.DeleteAsync($"/api/v1/transactions/{id}")).StatusCode);
        Assert.Equal(0, (await intruder.GetFromJsonAsync<JsonElement>("/api/v1/transactions")).GetArrayLength());
    }

    // REQ-004 (SPEC-001)
    [Fact]
    public async Task Balance_ReturnsIncomeMinusExpense()
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var salary = await ApiClient.CreateCategoryAsync(client, "Salário", "INCOME");
        var market = await ApiClient.CreateCategoryAsync(client, "Mercado");
        await ApiClient.CreateTransactionAsync(client, salary, 1000m, "INCOME");
        await ApiClient.CreateTransactionAsync(client, market, 250.50m);

        var balance = await client.GetFromJsonAsync<JsonElement>("/api/v1/transactions/balance");

        Assert.Equal(1000m, balance.GetProperty("totalIncome").GetDecimal());
        Assert.Equal(250.50m, balance.GetProperty("totalExpense").GetDecimal());
        Assert.Equal(749.50m, balance.GetProperty("balance").GetDecimal());
    }
}
