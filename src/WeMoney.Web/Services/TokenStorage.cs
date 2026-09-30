using Microsoft.JSInterop;

namespace WeMoney.Web.Services;

/// <summary>Persiste o access token no navegador (localStorage).</summary>
public class TokenStorage(IJSRuntime js)
{
    private const string Key = "wemoney.accessToken";

    public ValueTask<string?> GetAsync() => js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask SetAsync(string token) => js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask RemoveAsync() => js.InvokeVoidAsync("localStorage.removeItem", Key);
}
