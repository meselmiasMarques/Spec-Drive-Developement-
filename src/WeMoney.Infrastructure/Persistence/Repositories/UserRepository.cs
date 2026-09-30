using Microsoft.EntityFrameworkCore;
using WeMoney.Application.Abstractions;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence.Repositories;

public class UserRepository(WeMoneyDbContext db) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Email == email, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public void Add(User user) => db.Users.Add(user);
}
