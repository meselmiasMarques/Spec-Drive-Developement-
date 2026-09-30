namespace WeMoney.Web.Models;

// Contratos da API (SPEC-005). Duplicados aqui para manter o frontend desacoplado do backend.

public enum TransactionType
{
    INCOME,
    EXPENSE
}

public record RegisterRequest(string Name, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record UserResponse(Guid Id, string Name, string Email, DateTime CreatedAt);

public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserResponse User);

public record CategoryRequest(string Name, TransactionType Type);

public record CategoryResponse(Guid Id, string Name, TransactionType Type);

public record TransactionRequest(
    string Description,
    decimal Amount,
    TransactionType Type,
    Guid CategoryId,
    DateOnly? TransactionDate);

public record TransactionResponse(
    Guid Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    Guid CategoryId,
    DateOnly TransactionDate,
    DateTime CreatedAt);

public record BalanceResponse(decimal TotalIncome, decimal TotalExpense, decimal Balance);
