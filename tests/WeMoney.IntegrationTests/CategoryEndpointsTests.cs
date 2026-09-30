using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WeMoney.IntegrationTests;

public class CategoryEndpointsTests(WeMoneyApiFactory factory) : IClassFixture<WeMoneyApiFactory>
{
    [Fact]
    public async Task Crud_Works_ForOwner()
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);

        var create = await client.PostAsJsonAsync("/api/v1/categories", new { name = "Contas fixas", type = "EXPENSE" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var id = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();

        var update = await client.PutAsJsonAsync($"/api/v1/categories/{id}", new { name = "Moradia", type = "EXPENSE" });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var get = await client.GetFromJsonAsync<JsonElement>($"/api/v1/categories/{id}");
        Assert.Equal("Moradia", get.GetProperty("name").GetString());
        Assert.Equal("EXPENSE", get.GetProperty("type").GetString());

        var delete = await client.DeleteAsync($"/api/v1/categories/{id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/categories/{id}")).StatusCode);
    }

    [Theory]
    [InlineData("ab", "EXPENSE")]
    [InlineData("Casa", "OUTRO")]
    [InlineData("Casa", null)]
    public async Task Create_WithInvalidData_Returns400(string name, string? type)
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/v1/categories", new { name, type });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // RN-002
    [Fact]
    public async Task Delete_WithTransactions_Returns409()
    {
        var client = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var categoryId = await ApiClient.CreateCategoryAsync(client);
        await ApiClient.CreateTransactionAsync(client, categoryId);

        var response = await client.DeleteAsync($"/api/v1/categories/{categoryId}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // RN-003 / RN-004 / SPEC-006 — isolamento de dados
    [Fact]
    public async Task OtherUser_CannotSeeOrChangeCategory()
    {
        var owner = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var intruder = await ApiClient.CreateAuthenticatedClientAsync(factory);
        var id = await ApiClient.CreateCategoryAsync(owner, "Privada");

        Assert.Equal(HttpStatusCode.NotFound, (await intruder.GetAsync($"/api/v1/categories/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await intruder.PutAsJsonAsync($"/api/v1/categories/{id}", new { name = "Hack", type = "EXPENSE" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await intruder.DeleteAsync($"/api/v1/categories/{id}")).StatusCode);

        var intruderList = await intruder.GetFromJsonAsync<JsonElement>("/api/v1/categories");
        Assert.Equal(0, intruderList.GetArrayLength());

        var ownerView = await owner.GetFromJsonAsync<JsonElement>($"/api/v1/categories/{id}");
        Assert.Equal("Privada", ownerView.GetProperty("name").GetString());
    }
}
