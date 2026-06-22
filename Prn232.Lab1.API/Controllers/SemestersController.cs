using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/semesters")]
[Authorize]
[SwaggerTag("Semesters")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all semesters",
        Description = "Retrieve a paginated list of semesters with optional search, sort, fields, and expand options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllSemesters(
        [FromQuery, SwaggerParameter(Description = "Search by semester name")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. semesterName,-startDate")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. semesterId,semesterName")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. courses")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _semesterService.GetSemestersAsync(search, sort, page, resolvedPageSize, fields, expand);
        return Ok(ListApiHelper.ToListResponse(result, "Semesters retrieved successfully.", fields));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get semester details")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetSemesterById([FromRoute] int id)
    {
        var result = await _semesterService.GetSemesterByIdAsync(id);
        return Ok(ApiResult<SemesterResponseDto>.Ok(result, "Semester retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new semester")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateSemester(
        [FromBody, SwaggerParameter("New semester data to be created")] SemesterCreateRequestDto request)
    {
        var result = await _semesterService.CreateSemesterAsync(request);

        return CreatedAtAction(
            nameof(GetSemesterById),
            new { id = result.SemesterId },
            ApiResult<SemesterResponseDto>.Ok(result, "Semester created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update semester information")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> UpdateSemester(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated semester data")] SemesterUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("Semester update data is required."));
        }

        var result = await _semesterService.UpdateSemesterAsync(id, request);
        return Ok(ApiResult<SemesterResponseDto>.Ok(result, "Semester updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete a semester (Admin only)")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteSemester([FromRoute] int id)
    {
        await _semesterService.DeleteSemesterAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "Semester deleted successfully."));
    }
}
