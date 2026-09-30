using WeMoney.Domain.Entities;

namespace WeMoney.Application.Abstractions;

/// <summary>Todas as operações exigem o UserId do usuário autenticado (SPEC-006).</summary>
public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> ListAsync(Guid userId, CancellationToken ct = default);
    Task<Category?> GetAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<bool> HasTransactionsAsync(Guid id, Guid userId, CancellationToken ct = default);
    void Add(Category category);
    void Remove(Category category);
}
