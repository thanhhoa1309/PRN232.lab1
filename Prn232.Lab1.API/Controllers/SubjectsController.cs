using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;

    public SubjectsController(ISubjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetSubjectsAsync(search, sortBy, isDescending, page, pageSize);
        return Ok(ApiResult<Pagination<SubjectResponse>>.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSubject(int id)
    {
        var subject = await _service.GetSubjectByIdAsync(id);
        return Ok(ApiResult<SubjectResponse>.Success(subject));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateRequest request)
    {
        var created = await _service.CreateSubjectAsync(request);
        return CreatedAtAction(nameof(GetSubject), new { id = created.SubjectId }, ApiResult<SubjectResponse>.Success(created, "201", "Created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectUpdateRequest request)
    {
        var updated = await _service.UpdateSubjectAsync(id, request);
        return Ok(ApiResult<SubjectResponse>.Success(updated, "200", "Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        await _service.DeleteSubjectAsync(id);
        return Ok(ApiResult<object>.Success(null!, "200", "Deleted successfully"));
    }
}
