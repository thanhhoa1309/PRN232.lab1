using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
[Authorize]
[SwaggerTag("Courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all courses",
        Description = "Retrieve a paginated list of courses with optional search, sort, fields, and expand options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllCourses(
        [FromQuery, SwaggerParameter(Description = "Search by course name")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. courseName,-semesterId")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. courseId,courseName")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. semester")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _courseService.GetCoursesAsync(search, sort, page, resolvedPageSize, fields, expand);
        return Ok(ListApiHelper.ToListResponse(result, "Courses retrieved successfully.", fields));
    }

    [HttpGet("{id:int}/enrollments")]
    [SwaggerOperation(Summary = "Get enrollments of a course")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetEnrollmentsByCourse(
        [FromRoute] int id,
        [FromQuery] int page = 1,
        [FromQuery] int? size = null,
        [FromQuery] int limit = 10,
        [FromQuery, SwaggerParameter(Description = "Filter by status, e.g. Active")] string? status = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. -enrollDate,status")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Select fields")] string? fields = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, limit);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _courseService.GetEnrollmentsByCourseAsync(
            id, status, sort, page, resolvedPageSize, fields);

        return Ok(ListApiHelper.ToListResponse(result, $"Enrollments of course {id} retrieved successfully.", fields));
    }

    [HttpGet("{id:int}/students")]
    [SwaggerOperation(Summary = "Get students enrolled in a course")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetStudentsByCourse(
        [FromRoute] int id,
        [FromQuery, SwaggerParameter(Description = "Search by full name or email")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. fullName,-dateOfBirth")] string? sort = null,
        [FromQuery] int page = 1,
        [FromQuery] int? size = null,
        [FromQuery] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields")] string? fields = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _courseService.GetEnrolledStudentsByCourseAsync(
            id, search, sort, page, resolvedPageSize, fields);

        return Ok(ListApiHelper.ToListResponse(result, $"Students enrolled in course {id} retrieved successfully.", fields));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get course details")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetCourseByDetail([FromRoute] int id)
    {
        var result = await _courseService.GetCourseByDetailAsync(id);
        return Ok(ApiResult<CourseResponseDto>.Ok(result, "Course retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new course")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateCourse(
        [FromBody, SwaggerParameter("New course data to be created")] CourseCreateRequestDto request)
    {
        var result = await _courseService.CreateCourseAsync(request);

        return CreatedAtAction(
            nameof(GetCourseByDetail),
            new { id = result.CourseId, version = "1.0" },
            ApiResult<CourseResponseDto>.Ok(result, "Course created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update course information")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> UpdateCourse(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated course data")] CourseUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("Course update data is required."));
        }

        var result = await _courseService.UpdateCourseAsync(id, request);
        return Ok(ApiResult<CourseResponseDto>.Ok(result, "Course updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a course")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteCourse([FromRoute] int id)
    {
        await _courseService.DeleteCourseAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "Course deleted successfully."));
    }
}
