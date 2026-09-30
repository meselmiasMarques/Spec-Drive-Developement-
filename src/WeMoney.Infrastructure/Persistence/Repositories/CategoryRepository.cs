using Microsoft.EntityFrameworkCore;
using WeMoney.Application.Abstractions;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence.Repositories;

public class CategoryRepository(WeMoneyDbContext db) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> ListAsync(Guid userId, CancellationToken ct = default) =>
        await db.Categories.Where(c => c.UserId == userId).OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Category?> GetAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

    public Task<bool> HasTransactionsAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        db.Transactions.AnyAsync(t => t.CategoryId == id && t.UserId == userId, ct);

    public void Add(Category category) => db.Categories.Add(category);

    public void Remove(Category category) => db.Categories.Remove(category);
}
