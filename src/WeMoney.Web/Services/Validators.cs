using System.Net.Mail;

namespace WeMoney.Web.Services;

/// <summary>Validações de formulário espelhando as regras da SPEC. A API continua sendo a fonte da verdade.</summary>
public static class Validators
{
    public const int PasswordMinLength = 8;
    public const int CategoryNameMinLength = 3;
    public const int CategoryNameMaxLength = 100;
    public const int DescriptionMaxLength = 200;

    /// <summary>REQ-001: e-mail deve possuir formato válido.</summary>
    public static string? Email(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Obrigatoriedade tratada pelo campo.

        var email = value.Trim();
        return MailAddress.TryCreate(email, out var address) && address.Address == email
            ? null
            : "E-mail inválido.";
    }

    /// <summary>REQ-001 CA-003: senha com no mínimo 8 caracteres.</summary>
    public static string? Password(string? value) =>
        string.IsNullOrEmpty(value) || value.Length >= PasswordMinLength
            ? null
            : $"Senha deve possuir no mínimo {PasswordMinLength} caracteres.";

    /// <summary>REQ-008 / RN-001: nome da categoria entre 3 e 100 caracteres.</summary>
    public static string? CategoryName(string? value)
    {
        var length = value?.Trim().Length ?? 0;
        if (length == 0)
            return null;
        if (length < CategoryNameMinLength)
            return $"Nome deve ter no mínimo {CategoryNameMinLength} caracteres.";
        if (length > CategoryNameMaxLength)
            return $"Nome deve ter no máximo {CategoryNameMaxLength} caracteres.";
        return null;
    }

    /// <summary>Transações RN-001: valor maior que zero.</summary>
    public static string? Amount(decimal value) =>
        value > 0 ? null : "O valor deve ser maior que zero.";
}
