using WeMoney.Domain.Exceptions;

namespace WeMoney.Domain.Entities;

public class Transaction
{
    public const int DescriptionMaxLength = 200;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateOnly TransactionDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public Transaction(Guid userId, Category category, string description, decimal amount,
        TransactionType type, DateOnly transactionDate)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Usuário é obrigatório.");

        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Update(category, description, amount, type, transactionDate);
    }

    public void Update(Category category, string description, decimal amount,
        TransactionType type, DateOnly transactionDate)
    {
        // RN-002 / RN-003: categoria válida e pertencente ao mesmo usuário.
        if (category is null || category.UserId != UserId)
            throw new DomainException("Categoria inválida.");

        // RN-001: valor maior que zero.
        if (amount <= 0)
            throw new DomainException("O valor deve ser maior que zero.");

        // RN-005: tipo INCOME ou EXPENSE.
        if (!Enum.IsDefined(type))
            throw new DomainException("Tipo deve ser INCOME ou EXPENSE.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Descrição é obrigatória.");

        description = description.Trim();
        if (description.Length > DescriptionMaxLength)
            throw new DomainException($"Descrição deve ter no máximo {DescriptionMaxLength} caracteres.");

        CategoryId = category.Id;
        Description = description;
        Amount = amount;
        Type = type;
        TransactionDate = transactionDate;
    }
}
