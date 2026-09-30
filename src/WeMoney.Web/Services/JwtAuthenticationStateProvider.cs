using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WeMoney.Web.Services;

/// <summary>Estado de autenticação do frontend baseado no JWT da API (REQ-002 / REQ-003).</summary>
public class JwtAuthenticationStateProvider(TokenStorage storage, TimeProvider timeProvider)
    : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.GetAsync();
        var user = JwtParser.Parse(token, timeProvider.GetUtcNow());

        if (user is null)
        {
            // Token expirado ou inválido: descarta para não ser reenviado à API.
            if (token is not null)
                await storage.RemoveAsync();
            return Anonymous;
        }

        return new AuthenticationState(user);
    }

    public async Task SignInAsync(string token)
    {
        await storage.SetAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        await storage.RemoveAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
