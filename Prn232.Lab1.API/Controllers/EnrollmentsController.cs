using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/v2/enrollments")]
[ApiController]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    // =========================================================================
    // GET ALL  —  GET /api/enrollments
    // =========================================================================

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all enrollments",
        Description = "Retrieve a paginated list of enrollments with optional search, sort, and expand options.")]
    [ProducesResponseType(typeof(ApiResult<Pagination<EnrollmentResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<IActionResult> GetAllEnrollments(
        [FromQuery, SwaggerParameter(Description = "Search by enrollment status (optional)")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort by field: enrollmentId, studentId, courseId, enrollDate, status (optional)")] string? sortBy = null,
        [FromQuery, SwaggerParameter(Description = "Sort in descending order? Default: false")] bool isDescending = false,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. student, course (optional)")] string? expand = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Invalid pagination parameters."));
        }

        var result = await _enrollmentService.GetEnrollmentsAsync(search, sortBy, isDescending, page, pageSize, expand);

        return Ok(ApiResult<Pagination<EnrollmentResponseDto>>.Success(result, "200", "Enrollments retrieved successfully."));
    }

    // =========================================================================
    // GET BY ID  —  GET /api/enrollments/{id}
    // =========================================================================

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get enrollment details",
        Description = "Retrieve detailed information for a specific enrollment by ID.")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetEnrollmentById([FromRoute] int id)
    {
        var result = await _enrollmentService.GetEnrollmentByIdAsync(id);

        return Ok(ApiResult<EnrollmentResponseDto>.Success(result, "200", "Enrollment retrieved successfully."));
    }

    // =========================================================================
    // CREATE  —  POST /api/enrollments    // =========================================================================

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new enrollment",
        Description = "Creates a new enrollment with the provided information.")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody, SwaggerParameter("New enrollment data to be created")] EnrollmentCreateRequestDto request)
    {
        var result = await _enrollmentService.CreateEnrollmentAsync(request);

        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = result.EnrollmentId },
            ApiResult<EnrollmentResponseDto>.Success(result, "201", "Enrollment created successfully."));
    }

    // =========================================================================
    // UPDATE  —  PUT /api/enrollments/{id}         // =========================================================================

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update enrollment information",
        Description = "Updates the details of a specific enrollment by ID.")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> UpdateEnrollment(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated enrollment data")] EnrollmentUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Enrollment update data is required."));
        }

        var result = await _enrollmentService.UpdateEnrollmentAsync(id, request);

        return Ok(ApiResult<EnrollmentResponseDto>.Success(result, "200", "Enrollment updated successfully."));
    }

    // =========================================================================
    // DELETE  —  DELETE /api/enrollments/{id}      // =========================================================================

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete an enrollment",
        Description = "Deletes an enrollment by ID.")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> DeleteEnrollment([FromRoute] int id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);

        return Ok(ApiResult<bool>.Success(true, "200", "Enrollment deleted successfully."));
    }
}
