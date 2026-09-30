using WeMoney.Application.Auth;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Exceptions;
using WeMoney.UnitTests.Fakes;

namespace WeMoney.UnitTests.Application;

public class AuthServiceTests
{
    private readonly FakeUserRepository _users = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users, new FakePasswordHasher(), new FakeTokenService(), _unitOfWork);
    }

    // REQ-001 CA-001
    [Fact]
    public async Task Register_WithValidData_CreatesAccountWithHashedPassword()
    {
        var result = await _service.RegisterAsync(new RegisterRequest("João", "joao@email.com", "12345678"));

        var user = Assert.Single(_users.Items);
        Assert.Equal(user.Id, result.Id);
        Assert.NotEqual("12345678", user.PasswordHash);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    // REQ-001 CA-002
    [Fact]
    public async Task Register_WithExistingEmail_ThrowsConflict()
    {
        await _service.RegisterAsync(new RegisterRequest("João", "joao@email.com", "12345678"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegisterAsync(new RegisterRequest("Outro", "JOAO@email.com", "12345678")));
    }

    // REQ-001 CA-003
    [Fact]
    public async Task Register_WithShortPassword_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.RegisterAsync(new RegisterRequest("João", "joao@email.com", "1234567")));
        Assert.Empty(_users.Items);
    }

    // REQ-002 CA-001
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        await _service.RegisterAsync(new RegisterRequest("João", "joao@email.com", "12345678"));

        var result = await _service.LoginAsync(new LoginRequest("joao@email.com", "12345678"));

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.Equal("joao@email.com", result.User.Email);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ThrowsUnauthorized()
    {
        await _service.RegisterAsync(new RegisterRequest("João", "joao@email.com", "12345678"));

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.LoginAsync(new LoginRequest("joao@email.com", "errada123")));
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ThrowsUnauthorized()
    {
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.LoginAsync(new LoginRequest("ninguem@email.com", "12345678")));
    }

    [Theory]
    [InlineData(null, "12345678")]
    [InlineData("joao@email.com", null)]
    public async Task Login_WithMissingFields_Throws(string? email, string? password)
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.LoginAsync(new LoginRequest(email, password)));
    }
}
