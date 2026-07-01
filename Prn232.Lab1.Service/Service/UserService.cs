using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Users;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, PasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserResponseDto>> GetUsersAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        IQueryable<User> dbQuery = _unitOfWork.UserRepository.GetAllAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(u =>
                u.Username.Contains(keyword) || u.Role.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<User, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["userId"] = u => u.UserId,
            ["username"] = u => u.Username,
            ["role"] = u => u.Role
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sort, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        _ = fields;
        _ = expand;

        var responses = items.Select(MapToResponse).ToList();
        return PagedResult<UserResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("User not found.");
        }

        return MapToResponse(entity);
    }

    public async Task<UserResponseDto> CreateUserAsync(UserCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.Role))
        {
            throw ErrorHelper.BadRequest("Username, Password and Role are required.");
        }

        var username = request.Username.Trim();
        var exists = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Username == username);
        if (exists != null)
        {
            throw ErrorHelper.Conflict("Username already exists.");
        }

        var entity = new User
        {
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role.Trim()
        };

        await _unitOfWork.UserRepository.CreateAsync(entity);

        return MapToResponse(entity);
    }

    public async Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateRequestDto request)
    {
        var existing = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var username = request.Username.Trim();
            var duplicate = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
                u => u.Username == username && u.UserId != id);
            if (duplicate != null)
            {
                throw ErrorHelper.Conflict("Username already exists.");
            }

            existing.Username = username;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            existing.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            existing.Role = request.Role.Trim();
        }

        await _unitOfWork.UserRepository.UpdateAsync(existing);

        return MapToResponse(existing);
    }

    public async Task DeleteUserAsync(int id)
    {
        var existing = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("User not found.");
        }

        await _unitOfWork.UserRepository.RemoveAsync(existing);
    }

    private static UserResponseDto MapToResponse(User entity) => new()
    {
        UserId = entity.UserId,
        Username = entity.Username,
        Role = entity.Role
    };
}
