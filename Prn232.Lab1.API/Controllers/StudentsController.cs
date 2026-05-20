using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetStudentsAsync(search, sortBy, isDescending, page, pageSize, expand);
        return Ok(ApiResult<Pagination<StudentResponse>>.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        var student = await _service.GetStudentByIdAsync(id);
        return Ok(ApiResult<StudentResponse>.Success(student));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] StudentCreateRequest request)
    {
        var created = await _service.CreateStudentAsync(request);
        return CreatedAtAction(nameof(GetStudent), new { id = created.StudentId }, ApiResult<StudentResponse>.Success(created, "201", "Created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentUpdateRequest request)
    {
        var updated = await _service.UpdateStudentAsync(id, request);
        return Ok(ApiResult<StudentResponse>.Success(updated, "200", "Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        await _service.DeleteStudentAsync(id);
        return Ok(ApiResult<object>.Success(null!, "200", "Deleted successfully"));
    }
}
