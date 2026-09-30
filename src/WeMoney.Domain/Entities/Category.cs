using WeMoney.Domain.Exceptions;

namespace WeMoney.Domain.Entities;

public class Category
{
    public const int NameMinLength = 3;
    public const int NameMaxLength = 100;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public TransactionType Type { get; private set; }

    private Category() { }

    public Category(Guid userId, string name, TransactionType type)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Usuário é obrigatório.");

        Id = Guid.NewGuid();
        UserId = userId;
        Update(name, type);
    }

    public void Update(string name, TransactionType type)
    {
        Name = ValidateName(name);
        Type = ValidateType(type);
    }

    /// <summary>REQ-008 / RN-001: nome obrigatório, entre 3 e 100 caracteres.</summary>
    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");

        name = name.Trim();
        if (name.Length < NameMinLength)
            throw new DomainException($"Nome deve ter no mínimo {NameMinLength} caracteres.");
        if (name.Length > NameMaxLength)
            throw new DomainException($"Nome deve ter no máximo {NameMaxLength} caracteres.");

        return name;
    }

    private static TransactionType ValidateType(TransactionType type)
    {
        return Enum.IsDefined(type) ? type : throw new DomainException("Tipo deve ser INCOME ou EXPENSE.");
    }
}
