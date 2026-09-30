using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace WeMoney.Web.Services;

/// <summary>
/// Lê as claims do JWT emitido pela API. A assinatura é validada pela API em toda requisição;
/// aqui o token é apenas lido para replicar o estado de autenticação no frontend (REQ-002 CA-003).
/// </summary>
public static class JwtParser
{
    public const string AuthenticationType = "jwt";

    /// <summary>Retorna o usuário do token, ou null se o token for inválido ou estiver expirado.</summary>
    public static ClaimsPrincipal? Parse(string? token, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        try
        {
            using var payload = JsonDocument.Parse(DecodeBase64Url(parts[1]));
            var root = payload.RootElement;

            if (!root.TryGetProperty("exp", out var exp) || !exp.TryGetInt64(out var expSeconds))
                return null;
            if (DateTimeOffset.FromUnixTimeSeconds(expSeconds) <= now)
                return null;

            var claims = new List<Claim>();
            foreach (var name in new[] { "sub", "name", "email" })
            {
                if (root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String)
                    claims.Add(new Claim(name, value.GetString()!));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationType, "name", null));
        }
        catch (Exception e) when (e is FormatException or JsonException)
        {
            return null;
        }
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}
