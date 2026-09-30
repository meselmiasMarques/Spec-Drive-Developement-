using WeMoney.Domain.Entities;

namespace WeMoney.Application.Abstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) Generate(User user);
}
