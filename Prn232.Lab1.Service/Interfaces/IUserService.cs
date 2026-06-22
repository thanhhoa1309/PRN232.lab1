using Prn232.Lab1.Service.Dtos.Users;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetUsersAsync(
        string? search,
        string? sort,
        int page,
        int pageSize);

    Task<UserResponseDto> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(UserCreateRequestDto request);
    Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateRequestDto request);
    Task DeleteUserAsync(int id);
}
