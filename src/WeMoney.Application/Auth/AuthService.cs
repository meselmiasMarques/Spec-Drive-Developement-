using WeMoney.Application.Abstractions;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.Application.Auth;

public class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork)
{
    /// <summary>REQ-001 — Cadastro de usuário.</summary>
    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        User.ValidatePassword(request.Password);
        var email = User.NormalizeEmail(request.Email);

        if (await users.EmailExistsAsync(email, ct))
            throw new ConflictException("E-mail já cadastrado.");

        var user = new User(request.Name!, email, passwordHasher.Hash(request.Password!));
        users.Add(user);
        await unitOfWork.SaveChangesAsync(ct);

        return ToResponse(user);
    }

    /// <summary>REQ-002 — Login.</summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new DomainException("E-mail é obrigatório.");
        if (string.IsNullOrEmpty(request.Password))
            throw new DomainException("Senha é obrigatória.");

        var user = await users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("E-mail ou senha inválidos.");

        var (token, expiresAt) = tokenService.Generate(user);
        return new LoginResponse(token, expiresAt, ToResponse(user));
    }

    private static UserResponse ToResponse(User user) => new(user.Id, user.Name, user.Email, user.CreatedAt);
}
