using Microsoft.EntityFrameworkCore;
using WeMoney.Application.Abstractions;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence.Repositories;

public class TransactionRepository(WeMoneyDbContext db) : ITransactionRepository
{
    public async Task<IReadOnlyList<Transaction>> ListAsync(Guid userId, CancellationToken ct = default) =>
        await db.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

    public Task<Transaction?> GetAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

    public async Task<decimal> SumAsync(Guid userId, TransactionType type, CancellationToken ct = default) =>
        await db.Transactions
            .Where(t => t.UserId == userId && t.Type == type)
            .SumAsync(t => (decimal?)t.Amount, ct) ?? 0m;

    public void Add(Transaction transaction) => db.Transactions.Add(transaction);

    public void Remove(Transaction transaction) => db.Transactions.Remove(transaction);
}
