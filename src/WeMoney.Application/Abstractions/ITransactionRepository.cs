using WeMoney.Domain.Entities;

namespace WeMoney.Application.Abstractions;

/// <summary>Todas as operações exigem o UserId do usuário autenticado (SPEC-006).</summary>
public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> ListAsync(Guid userId, CancellationToken ct = default);
    Task<Transaction?> GetAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<decimal> SumAsync(Guid userId, TransactionType type, CancellationToken ct = default);
    void Add(Transaction transaction);
    void Remove(Transaction transaction);
}
