using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Auth;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/v2/auth")]
[ApiController]
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
    [SwaggerOperation(Summary = "Login and receive JWT tokens")]
    [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 401)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request, _configuration);
        return Ok(ApiResult<LoginResponseDto>.Success(result!, "200", "Login successful."));
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Get new access token using refresh token")]
    [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 401)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, _configuration);
        return Ok(ApiResult<LoginResponseDto>.Success(result!, "200", "Token refreshed successfully."));
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register new user (Admin only)")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
    {
        var result = await _authService.RegisterUserAsync(request);
        return StatusCode(201, ApiResult<UserDto>.Success(result!, "201", "User registered successfully."));
    }
}
