using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/semesters")]
[ApiController]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    // =========================================================================
    // GET ALL  —  GET /api/semesters
    // =========================================================================

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all semesters",
        Description = "Retrieve a paginated list of semesters with optional search, sort, and expand options.")]
    [ProducesResponseType(typeof(ApiResult<Pagination<SemesterResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<IActionResult> GetAllSemesters(
        [FromQuery, SwaggerParameter(Description = "Search by semester name (optional)")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort by field: semesterId, semesterName, startDate, endDate (optional)")] string? sortBy = null,
        [FromQuery, SwaggerParameter(Description = "Sort in descending order? Default: false")] bool isDescending = false,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. courses (optional)")] string? expand = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Invalid pagination parameters."));
        }

        var result = await _semesterService.GetSemestersAsync(search, sortBy, isDescending, page, pageSize, expand);

        return Ok(ApiResult<Pagination<SemesterResponse>>.Success(result, "200", "Semesters retrieved successfully."));
    }

    // =========================================================================
    // GET BY ID  —  GET /api/semesters/{id}
    // =========================================================================

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get semester details",
        Description = "Retrieve detailed information for a specific semester by ID.")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetSemesterById([FromRoute] int id)
    {
        var result = await _semesterService.GetSemesterByIdAsync(id);

        return Ok(ApiResult<SemesterResponse>.Success(result, "200", "Semester retrieved successfully."));
    }

    // =========================================================================
    // CREATE  —  POST /api/semesters    // =========================================================================

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new semester",
        Description = "Creates a new semester with the provided information.")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponse>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> CreateSemester(
        [FromBody, SwaggerParameter("New semester data to be created")] SemesterCreateRequest request)
    {
        var result = await _semesterService.CreateSemesterAsync(request);

        return CreatedAtAction(
            nameof(GetSemesterById),
            new { id = result.SemesterId },
            ApiResult<SemesterResponse>.Success(result, "201", "Semester created successfully."));
    }

    // =========================================================================
    // UPDATE  —  PUT /api/semesters/{id}         // =========================================================================

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update semester information",
        Description = "Updates the details of a specific semester by ID.")]
    [ProducesResponseType(typeof(ApiResult<SemesterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> UpdateSemester(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated semester data")] SemesterUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Semester update data is required."));
        }

        var result = await _semesterService.UpdateSemesterAsync(id, request);

        return Ok(ApiResult<SemesterResponse>.Success(result, "200", "Semester updated successfully."));
    }

    // =========================================================================
    // DELETE  —  DELETE /api/semesters/{id}      // =========================================================================

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a semester",
        Description = "Deletes a semester by ID.")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> DeleteSemester([FromRoute] int id)
    {
        await _semesterService.DeleteSemesterAsync(id);

        return Ok(ApiResult<bool>.Success(true, "200", "Semester deleted successfully."));
    }
}
