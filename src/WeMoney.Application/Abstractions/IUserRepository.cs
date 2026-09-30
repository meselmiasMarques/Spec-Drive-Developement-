using WeMoney.Domain.Entities;

namespace WeMoney.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    void Add(User user);
}
