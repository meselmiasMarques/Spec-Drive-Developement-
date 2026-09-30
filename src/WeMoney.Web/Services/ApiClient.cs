using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using WeMoney.Web.Models;

namespace WeMoney.Web.Services;

/// <summary>Cliente da API WeMoney (SPEC-005). Envia o JWT e trata erros de forma centralizada.</summary>
public class ApiClient(
    HttpClient http,
    TokenStorage tokens,
    JwtAuthenticationStateProvider authState,
    NavigationManager navigation)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    // Auth
    public Task RegisterAsync(RegisterRequest request) =>
        SendAsync(HttpMethod.Post, "api/v1/auth/register", request);

    public Task<LoginResponse> LoginAsync(LoginRequest request) =>
        SendAsync<LoginResponse>(HttpMethod.Post, "api/v1/auth/login", request);

    // Categorias
    public Task<List<CategoryResponse>> GetCategoriesAsync() =>
        SendAsync<List<CategoryResponse>>(HttpMethod.Get, "api/v1/categories");

    public Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request) =>
        SendAsync<CategoryResponse>(HttpMethod.Post, "api/v1/categories", request);

    public Task<CategoryResponse> UpdateCategoryAsync(Guid id, CategoryRequest request) =>
        SendAsync<CategoryResponse>(HttpMethod.Put, $"api/v1/categories/{id}", request);

    public Task DeleteCategoryAsync(Guid id) =>
        SendAsync(HttpMethod.Delete, $"api/v1/categories/{id}");

    // Transações
    public Task<List<TransactionResponse>> GetTransactionsAsync() =>
        SendAsync<List<TransactionResponse>>(HttpMethod.Get, "api/v1/transactions");

    public Task<TransactionResponse> CreateTransactionAsync(TransactionRequest request) =>
        SendAsync<TransactionResponse>(HttpMethod.Post, "api/v1/transactions", request);

    public Task<TransactionResponse> UpdateTransactionAsync(Guid id, TransactionRequest request) =>
        SendAsync<TransactionResponse>(HttpMethod.Put, $"api/v1/transactions/{id}", request);

    public Task DeleteTransactionAsync(Guid id) =>
        SendAsync(HttpMethod.Delete, $"api/v1/transactions/{id}");

    public Task<BalanceResponse> GetBalanceAsync() =>
        SendAsync<BalanceResponse>(HttpMethod.Get, "api/v1/transactions/balance");

    private async Task<T> SendAsync<T>(HttpMethod method, string uri, object? body = null)
    {
        using var response = await SendCoreAsync(method, uri, body);
        return await response.Content.ReadFromJsonAsync<T>(Json)
               ?? throw new ApiException("Resposta vazia da API.");
    }

    private async Task SendAsync(HttpMethod method, string uri, object? body = null)
    {
        using var response = await SendCoreAsync(method, uri, body);
    }

    private async Task<HttpResponseMessage> SendCoreAsync(HttpMethod method, string uri, object? body)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: Json);

        var token = await tokens.GetAsync();
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ApiException("Não foi possível conectar à API.");
        }

        if (response.IsSuccessStatusCode)
            return response;

        using (response)
        {
            // Sessão expirada ou token rejeitado: volta para o login (REQ-003 CA-001).
            if (response.StatusCode == HttpStatusCode.Unauthorized && token is not null)
            {
                await authState.SignOutAsync();
                navigation.NavigateTo("login");
                throw new ApiException("Sessão expirada. Faça login novamente.");
            }

            var content = await response.Content.ReadAsStringAsync();
            throw new ApiException(ProblemDetailsReader.GetMessage(content, response.StatusCode));
        }
    }
}
