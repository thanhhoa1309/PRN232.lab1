using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnrollments(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetEnrollmentsAsync(search, sortBy, isDescending, page, pageSize, expand);
        return Ok(ApiResult<Pagination<EnrollmentResponse>>.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEnrollment(int id)
    {
        var enrollment = await _service.GetEnrollmentByIdAsync(id);
        return Ok(ApiResult<EnrollmentResponse>.Success(enrollment));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentCreateRequest request)
    {
        var created = await _service.CreateEnrollmentAsync(request);
        return CreatedAtAction(nameof(GetEnrollment), new { id = created.EnrollmentId }, ApiResult<EnrollmentResponse>.Success(created, "201", "Created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentUpdateRequest request)
    {
        var updated = await _service.UpdateEnrollmentAsync(id, request);
        return Ok(ApiResult<EnrollmentResponse>.Success(updated, "200", "Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        await _service.DeleteEnrollmentAsync(id);
        return Ok(ApiResult<object>.Success(null!, "200", "Deleted successfully"));
    }
}
