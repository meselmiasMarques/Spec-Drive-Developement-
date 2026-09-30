using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace WeMoney.Api.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    /// <summary>UserId do usuário autenticado, usado em toda consulta (SPEC-006).</summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAccessException("Token sem identificação de usuário.");
    }
}
