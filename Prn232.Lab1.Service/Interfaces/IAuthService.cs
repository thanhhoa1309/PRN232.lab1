using Microsoft.Extensions.Configuration;
using Prn232.Lab1.Service.Dtos.Auth;


namespace Prn232.Lab1.Service.Interfaces;

public interface IAuthService
{
    Task<UserDto?> RegisterUserAsync(UserRegistrationDto registrationDto);

    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration);

    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken, IConfiguration configuration);

}
