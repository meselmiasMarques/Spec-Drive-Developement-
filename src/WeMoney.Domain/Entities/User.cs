using System.Net.Mail;
using WeMoney.Domain.Exceptions;

namespace WeMoney.Domain.Entities;

public class User
{
    public const int NameMaxLength = 150;
    public const int EmailMaxLength = 200;
    public const int PasswordMinLength = 8;

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string name, string email, string passwordHash)
    {
        Name = ValidateName(name);
        Email = NormalizeEmail(email);
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash)
            ? throw new DomainException("Hash da senha é obrigatório.")
            : passwordHash;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>REQ-001: e-mail obrigatório e com formato válido.</summary>
    public static string NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");

        var normalized = email.Trim().ToLowerInvariant();
        if (normalized.Length > EmailMaxLength || !IsValidEmail(normalized))
            throw new DomainException("E-mail inválido.");

        return normalized;
    }

    /// <summary>REQ-001: senha com no mínimo 8 caracteres.</summary>
    public static void ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < PasswordMinLength)
            throw new DomainException($"Senha deve possuir no mínimo {PasswordMinLength} caracteres.");
    }

    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");

        name = name.Trim();
        if (name.Length > NameMaxLength)
            throw new DomainException($"Nome deve ter no máximo {NameMaxLength} caracteres.");

        return name;
    }

    private static bool IsValidEmail(string email)
    {
        return MailAddress.TryCreate(email, out var address)
               && address.Address == email
               && address.Host.Contains('.');
    }
}
