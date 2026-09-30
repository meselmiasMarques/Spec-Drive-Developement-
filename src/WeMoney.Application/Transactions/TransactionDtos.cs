using WeMoney.Domain.Entities;

namespace WeMoney.Application.Transactions;

public record TransactionRequest(
    string? Description,
    decimal Amount,
    TransactionType? Type,
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
