using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/semesters")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;

    public SemestersController(ISemesterService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSemesters(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetSemestersAsync(search, sortBy, isDescending, page, pageSize, expand);
        return Ok(ApiResult<Pagination<SemesterResponse>>.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSemester(int id)
    {
        var semester = await _service.GetSemesterByIdAsync(id);
        return Ok(ApiResult<SemesterResponse>.Success(semester));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSemester([FromBody] SemesterCreateRequest request)
    {
        var created = await _service.CreateSemesterAsync(request);
        return CreatedAtAction(nameof(GetSemester), new { id = created.SemesterId }, ApiResult<SemesterResponse>.Success(created, "201", "Created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterUpdateRequest request)
    {
        var updated = await _service.UpdateSemesterAsync(id, request);
        return Ok(ApiResult<SemesterResponse>.Success(updated, "200", "Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSemester(int id)
    {
        await _service.DeleteSemesterAsync(id);
        return Ok(ApiResult<object>.Success(null!, "200", "Deleted successfully"));
    }
}
