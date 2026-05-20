using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetCoursesAsync(search, sortBy, isDescending, page, pageSize);
        return Ok(ApiResult<Pagination<CourseResponse>>.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCourse(int id, [FromQuery] string? expand = null)
    {
        var course = await _service.GetCourseByIdAsync(id, expand);
        return Ok(ApiResult<CourseResponse>.Success(course));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateRequest request)
    {
        var created = await _service.CreateCourseAsync(request);
        return CreatedAtAction(nameof(GetCourse), new { id = created.CourseId }, ApiResult<CourseResponse>.Success(created, "201", "Created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateRequest request)
    {
        var updated = await _service.UpdateCourseAsync(id, request);
        return Ok(ApiResult<CourseResponse>.Success(updated, "200", "Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        await _service.DeleteCourseAsync(id);
        return Ok(ApiResult<object>.Success(null!, "200", "Deleted successfully"));
    }
}
