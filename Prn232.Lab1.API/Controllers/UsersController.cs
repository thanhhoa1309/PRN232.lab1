using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Users;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
[SwaggerTag("Users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Retrieve a paginated list of users with optional search, sort, fields, and expand options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery, SwaggerParameter(Description = "Search by username or role")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. username,-role")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. userId,username,role")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data (no relations for User)")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _userService.GetUsersAsync(
            search, sort, page, resolvedPageSize, fields, expand);
        return Ok(ListApiHelper.ToListResponse(result, "Users retrieved successfully.", fields));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [ProducesResponseType(typeof(ApiResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return Ok(ApiResult<UserResponseDto>.Ok(result, "User retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [ProducesResponseType(typeof(ApiResult<UserResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequestDto request)
    {
        var result = await _userService.CreateUserAsync(request);
        return CreatedAtAction(
            nameof(GetUserById),
            new { id = result.UserId },
            ApiResult<UserResponseDto>.Ok(result, "User created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update user")]
    [ProducesResponseType(typeof(ApiResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("User update data is required."));
        }

        var result = await _userService.UpdateUserAsync(id, request);
        return Ok(ApiResult<UserResponseDto>.Ok(result, "User updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete user (Admin only)")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "User deleted successfully."));
    }
}
