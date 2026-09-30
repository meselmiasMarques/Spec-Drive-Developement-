using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Application.Auth;

namespace WeMoney.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>REQ-001 — Cadastro de usuário.</summary>
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var user = await authService.RegisterAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    /// <summary>REQ-002 — Login.</summary>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        return await authService.LoginAsync(request, ct);
    }
}
