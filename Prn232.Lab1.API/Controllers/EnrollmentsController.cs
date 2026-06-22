using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
[SwaggerTag("Enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all enrollments",
        Description = "Retrieve a paginated list of enrollments with optional search, sort, fields, and expand options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllEnrollments(
        [FromQuery, SwaggerParameter(Description = "Search by enrollment status")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. -enrollDate,status")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. enrollmentId,status")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. student,course")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _enrollmentService.GetEnrollmentsAsync(search, sort, page, resolvedPageSize, fields, expand);
        return Ok(ListApiHelper.ToListResponse(result, "Enrollments retrieved successfully.", fields));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get enrollment details")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetEnrollmentById([FromRoute] int id)
    {
        var result = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Ok(ApiResult<EnrollmentResponseDto>.Ok(result, "Enrollment retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new enrollment")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody, SwaggerParameter("New enrollment data to be created")] EnrollmentCreateRequestDto request)
    {
        var result = await _enrollmentService.CreateEnrollmentAsync(request);

        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = result.EnrollmentId },
            ApiResult<EnrollmentResponseDto>.Ok(result, "Enrollment created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update enrollment information")]
    [ProducesResponseType(typeof(ApiResult<EnrollmentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> UpdateEnrollment(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated enrollment data")] EnrollmentUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("Enrollment update data is required."));
        }

        var result = await _enrollmentService.UpdateEnrollmentAsync(id, request);
        return Ok(ApiResult<EnrollmentResponseDto>.Ok(result, "Enrollment updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete an enrollment")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteEnrollment([FromRoute] int id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "Enrollment deleted successfully."));
    }
}
