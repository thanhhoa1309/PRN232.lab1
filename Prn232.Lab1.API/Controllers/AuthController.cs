using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Auth;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/auth")]
[Route("api/v{version:apiVersion}/auth")]
[SwaggerTag("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login and receive JWT tokens")]
    [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 401)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request, _configuration);
        return Ok(ApiResult<LoginResponseDto>.Ok(result!, "Login successful."));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get new access token using refresh token")]
    [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 401)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, _configuration);
        return Ok(ApiResult<LoginResponseDto>.Ok(result!, "Token refreshed successfully."));
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Register new user (Admin only)")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
    {
        var result = await _authService.RegisterUserAsync(request);
        return StatusCode(201, ApiResult<UserDto>.Ok(result!, "User registered successfully."));
    }
}
