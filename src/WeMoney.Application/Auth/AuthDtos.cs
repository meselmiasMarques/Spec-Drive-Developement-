namespace WeMoney.Application.Auth;

public record RegisterRequest(string? Name, string? Email, string? Password);

public record LoginRequest(string? Email, string? Password);

public record UserResponse(Guid Id, string Name, string Email, DateTime CreatedAt);

public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserResponse User);
