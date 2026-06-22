using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Auth;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Security.Cryptography;

namespace Prn232.Lab1.Service.Service;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _loggerService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        IUnitOfWork unitOfWork,
        ILogger<AuthService> loggerService,
        PasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _loggerService = loggerService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    ///     Register a new user.
    /// </summary>
    /// <param name="registrationDto"></param>
    /// <returns></returns>
    public async Task<UserDto?> RegisterUserAsync(UserRegistrationDto registrationDto)
    {
        _loggerService.LogInformation("Start registration for {Username}", registrationDto.Username);

        if (await UserExistsAsync(registrationDto.Username))
            throw ErrorHelper.Conflict("Username have been used.");

        var user = new User
        {
            Username = registrationDto.Username,
            PasswordHash = _passwordHasher.HashPassword(registrationDto.Password),
            Role = registrationDto.Role
        };

        await _unitOfWork.UserRepository.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }

    /// <summary>
    ///     Login a user and return JWT access and refresh token.
    /// </summary>
    /// <param name="loginDto"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration)
    {
        _loggerService.LogInformation("Login attempt for {Username}", loginDto.Username);

        var user = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
            throw ErrorHelper.NotFound("Account does not exist.");

        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw ErrorHelper.Unauthorized("Password is incorrect.");

        var accessToken = JwtUtils.GenerateJwtToken(
            user.UserId, user.Username, user.Role,
            configuration, AuthTokenConstants.AccessTokenValidity);

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        await _unitOfWork.UserRepository.UpdateAsync(user);

        _loggerService.LogInformation("User {Username} logged in successfully.", user.Username);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = AuthTokenConstants.AccessTokenExpiresInSeconds
        };
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken, IConfiguration configuration)
    {
        var user = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
            throw ErrorHelper.Unauthorized("Invalid or expired refresh token.");

        var newAccessToken = JwtUtils.GenerateJwtToken(
            user.UserId, user.Username, user.Role,
            configuration, AuthTokenConstants.AccessTokenValidity);

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _unitOfWork.UserRepository.UpdateAsync(user);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = AuthTokenConstants.AccessTokenExpiresInSeconds
        };
    }


    //========================= PRIVATE HELPER METHODS ============================

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task<bool> UserExistsAsync(string username)
    {
        var user = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Username == username);
        return user != null;
    }
}
