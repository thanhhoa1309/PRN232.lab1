using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Auth;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Service;

public class AuthService : IAuthService
{
    private readonly ILogger _loggerService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUnitOfWork unitOfWork,
        ILogger<AuthService> loggerService)
    {
        _unitOfWork = unitOfWork;
        _loggerService = loggerService;
    }

    /// <summary>
    ///     Register a new user.
    /// </summary>
    /// <param name="registrationDto"></param>
    /// <returns></returns>
    public async Task<UserDto?> RegisterUserAsync(UserRegistrationDto registrationDto)
    {
        _loggerService.LogInformation($"Start registration for {registrationDto.Username}");

        if (await UserExistsAsync(registrationDto.Username))
        {
            _loggerService.LogWarning($"Username {registrationDto.Username} already registered.");
            throw ErrorHelper.Conflict("Username have been used.");
        }

        var hashedPassword = new PasswordHasher().HashPassword(registrationDto.Password);

        var user = new User
        {
            Username = registrationDto.Username,
            PasswordHash = hashedPassword,
            Role = registrationDto.Role
        };

        await _unitOfWork.UserRepository.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _loggerService.LogInformation($"User {user.Username} created successfully.");

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
        _loggerService.LogInformation($"Login attempt for {loginDto.Username}");

        // Get user from DB
        var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
            throw ErrorHelper.NotFound("Account does not exist.");

        if (!new PasswordHasher().VerifyPassword(loginDto.Password!, user.PasswordHash))
            throw ErrorHelper.Unauthorized("Password is incorrect.");

        _loggerService.LogInformation($"User {loginDto.Username} authenticated successfully.");

        // Generate JWT token and refresh token
        var accessToken = JwtUtils.GenerateJwtToken(
            user.UserId,
            user.Username,
            user.Role,
            configuration,
            TimeSpan.FromMinutes(30)
        );

        _loggerService.LogInformation($"Access token generated for {user.Username}");

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken ?? string.Empty
        };
    }


    //========================= PRIVATE HELPER METHODS ============================

    private async Task<bool> UserExistsAsync(string username)
    {
        var existingUser = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Username == username);
        return existingUser != null;
    }
}
