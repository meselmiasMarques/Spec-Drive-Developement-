using WeMoney.Application.Abstractions;
using WeMoney.Domain.Entities;

namespace WeMoney.UnitTests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        SaveCount++;
        return Task.CompletedTask;
    }
}

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";
    public bool Verify(string password, string passwordHash) => passwordHash == $"hashed:{password}";
}

public class FakeTokenService : ITokenService
{
    public (string Token, DateTime ExpiresAt) Generate(User user) => ($"token-{user.Id}", DateTime.UtcNow.AddMinutes(30));
}

public class FakeUserRepository : IUserRepository
{
    public List<User> Items { get; } = [];

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(Items.Any(u => u.Email == email));

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(u => u.Email == email));

    public void Add(User user) => Items.Add(user);
}

public class FakeCategoryRepository(FakeTransactionRepository? transactions = null) : ICategoryRepository
{
    public List<Category> Items { get; } = [];

    public Task<IReadOnlyList<Category>> ListAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Category>>(Items.Where(c => c.UserId == userId).ToList());

    public Task<Category?> GetAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(c => c.Id == id && c.UserId == userId));

    public Task<bool> HasTransactionsAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        Task.FromResult(transactions?.Items.Any(t => t.CategoryId == id && t.UserId == userId) ?? false);

    public void Add(Category category) => Items.Add(category);
    public void Remove(Category category) => Items.Remove(category);
}

public class FakeTransactionRepository : ITransactionRepository
{
    public List<Transaction> Items { get; } = [];

    public Task<IReadOnlyList<Transaction>> ListAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Transaction>>(Items.Where(t => t.UserId == userId).ToList());

    public Task<Transaction?> GetAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(t => t.Id == id && t.UserId == userId));

    public Task<decimal> SumAsync(Guid userId, TransactionType type, CancellationToken ct = default) =>
        Task.FromResult(Items.Where(t => t.UserId == userId && t.Type == type).Sum(t => t.Amount));

    public void Add(Transaction transaction) => Items.Add(transaction);
    public void Remove(Transaction transaction) => Items.Remove(transaction);
}

public class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
