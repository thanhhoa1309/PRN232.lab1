using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/students")]
[Authorize]
[SwaggerTag("Students v1")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all students",
        Description = "Retrieve a paginated list of students with optional search, sort, fields, and expand options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllStudents(
        [FromQuery, SwaggerParameter(Description = "Search by full name or email")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. fullName,-dateOfBirth")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. studentId,fullName,email")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. enrollments")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _studentService.GetStudentsAsync(
            search, sort, page, resolvedPageSize, fields, expand);

        return Ok(ListApiHelper.ToListResponse(result, "Students retrieved successfully.", fields));
    }

    [HttpGet("{id:int}", Name = "GetStudentById")]
    [SwaggerOperation(Summary = "Get student details")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetStudentById(
        [FromRoute] int id,
        [FromHeader(Name = "X-Request-Id")] string? requestId = null)
    {
        if (!string.IsNullOrWhiteSpace(requestId))
        {
            _logger.LogInformation("GetStudentById request {RequestId} for student {StudentId}", requestId, id);
        }

        var result = await _studentService.GetStudentByIdAsync(id);
        return Ok(ApiResult<StudentResponseDto>.Ok(result, "Student retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new student")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateStudent(
        [FromBody, SwaggerParameter("New student data to be created")] StudentCreateRequestDto request)
    {
        var result = await _studentService.CreateStudentAsync(request);

        return CreatedAtRoute(
            "GetStudentById",
            new { id = result.StudentId, version = "1.0" },
            ApiResult<StudentResponseDto>.Ok(result, "Student created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update student information")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> UpdateStudent(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated student data")] StudentUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("Student update data is required."));
        }

        var result = await _studentService.UpdateStudentAsync(id, request);
        return Ok(ApiResult<StudentResponseDto>.Ok(result, "Student updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a student")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteStudent([FromRoute] int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "Student deleted successfully."));
    }
}
