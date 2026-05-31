using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/v2/students")]
[ApiController]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // =========================================================================
    // GET ALL  —  GET /api/students
    // =========================================================================

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all students",
        Description = "Retrieve a paginated list of students with optional search, sort, and expand options.")]
    [ProducesResponseType(typeof(ApiResult<Pagination<StudentResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<IActionResult> GetAllStudents(
        [FromQuery, SwaggerParameter(Description = "Search by full name or email (optional)")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort by field: studentId, fullName, email, dateOfBirth (optional)")] string? sortBy = null,
        [FromQuery, SwaggerParameter(Description = "Sort in descending order? Default: false")] bool isDescending = false,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Expand related data, e.g. enrollments (optional)")] string? expand = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Invalid pagination parameters."));
        }

        var result = await _studentService.GetStudentsAsync(search, sortBy, isDescending, page, pageSize, expand);

        return Ok(ApiResult<Pagination<StudentResponseDto>>.Success(result, "200", "Students retrieved successfully."));
    }

    // =========================================================================
    // GET BY ID  —  GET /api/students/{id}
    // =========================================================================

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get student details",
        Description = "Retrieve detailed information for a specific student by ID.")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetStudentById([FromRoute] int id)
    {
        var result = await _studentService.GetStudentByIdAsync(id);

        return Ok(ApiResult<StudentResponseDto>.Success(result, "200", "Student retrieved successfully."));
    }

    // =========================================================================
    // CREATE  —  POST /api/students    // =========================================================================

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new student",
        Description = "Creates a new student with the provided information.")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> CreateStudent(
        [FromBody, SwaggerParameter("New student data to be created")] StudentCreateRequestDto request)
    {
        var result = await _studentService.CreateStudentAsync(request);

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = result.StudentId },
            ApiResult<StudentResponseDto>.Success(result, "201", "Student created successfully."));
    }

    // =========================================================================
    // UPDATE  —  PUT /api/students/{id}         // =========================================================================

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update student information",
        Description = "Updates the details of a specific student by ID.")]
    [ProducesResponseType(typeof(ApiResult<StudentResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> UpdateStudent(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated student data")] StudentUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Student update data is required."));
        }

        var result = await _studentService.UpdateStudentAsync(id, request);

        return Ok(ApiResult<StudentResponseDto>.Success(result, "200", "Student updated successfully."));
    }

    // =========================================================================
    // DELETE  —  DELETE /api/students/{id}      // =========================================================================

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a student",
        Description = "Deletes a student by ID.")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> DeleteStudent([FromRoute] int id)
    {
        await _studentService.DeleteStudentAsync(id);

        return Ok(ApiResult<bool>.Success(true, "200", "Student deleted successfully."));
    }
}
