namespace WeMoney.Infrastructure.Security;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Issuer { get; set; } = "WeMoney";
    public string Audience { get; set; } = "WeMoney";

    /// <summary>Segredo fornecido por variável de ambiente / user-secrets (nunca no código-fonte).</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 30;
}
