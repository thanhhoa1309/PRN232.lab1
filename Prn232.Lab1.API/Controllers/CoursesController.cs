using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/courses")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // =========================================================================
    // GET ALL  —  GET /api/courses
    // =========================================================================

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all courses",
        Description = "Retrieve a paginated list of courses with optional search, sort, and expand options.")]
    [ProducesResponseType(typeof(ApiResult<Pagination<CourseResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<IActionResult> GetAllCourses(
        [FromQuery, SwaggerParameter(Description = "Search by course name (optional)")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort by field: courseId, courseName, semesterId (optional)")] string? sortBy = null,
        [FromQuery, SwaggerParameter(Description = "Sort in descending order? Default: false")] bool isDescending = false,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. semester (optional)")] string? expand = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Invalid pagination parameters."));
        }

        var result = await _courseService.GetCoursesAsync(search, sortBy, isDescending, page, pageSize, expand);

        return Ok(ApiResult<Pagination<CourseResponseDto>>.Success(result, "200", "Courses retrieved successfully."));
    }

    // =========================================================================
    // GET BY ID  —  GET /api/courses/{id}
    // =========================================================================

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get course details",
        Description = "Retrieve detailed information for a specific course by ID, including semester.")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetCourseByDetail([FromRoute] int id)
    {
        var result = await _courseService.GetCourseByDetailAsync(id);

        return Ok(ApiResult<CourseResponseDto>.Success(result, "200", "Course retrieved successfully."));
    }

    // =========================================================================
    // GET ENROLLMENT BY COURSE  —  GET /api/courses/enrollment/{id}
    // =========================================================================

    [HttpGet("enrollment/{id:int}")]
    [SwaggerOperation(
        Summary = "Get course details",
        Description = "Retrieve course by ID, including enrolled students and enrollments.")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetEnrollmentByCourse([FromRoute] int id)
    {
        var result = await _courseService.GetEnrollmentByCourseAsync(id);

        return Ok(ApiResult<CourseResponseDto>.Success(result, "200", "Course retrieved successfully."));
    }

    // =========================================================================
    // CREATE  —  POST /api/courses    // =========================================================================

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new course",
        Description = "Creates a new course with the provided information.")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> CreateCourse(
        [FromBody, SwaggerParameter("New course data to be created")] CourseCreateRequestDto request)
    {
        var result = await _courseService.CreateCourseAsync(request);

        return CreatedAtAction(
            nameof(GetCourseByDetail),
            new { id = result.CourseId },
            ApiResult<CourseResponseDto>.Success(result, "201", "Course created successfully."));
    }

    // =========================================================================
    // UPDATE  —  PUT /api/courses/{id}         // =========================================================================

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update course information",
        Description = "Updates the details of a specific course by ID.")]
    [ProducesResponseType(typeof(ApiResult<CourseResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> UpdateCourse(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated course data")] CourseUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Course update data is required."));
        }

        var result = await _courseService.UpdateCourseAsync(id, request);

        return Ok(ApiResult<CourseResponseDto>.Success(result, "200", "Course updated successfully."));
    }

    // =========================================================================
    // DELETE  —  DELETE /api/courses/{id}      // =========================================================================

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a course",
        Description = "Deletes a course by ID.")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> DeleteCourse([FromRoute] int id)
    {
        await _courseService.DeleteCourseAsync(id);

        return Ok(ApiResult<bool>.Success(true, "200", "Course deleted successfully."));
    }
}
