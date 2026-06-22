using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v2/students")]
[Authorize]
[SwaggerTag("Students v2")]
public class StudentsV2Controller : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsV2Controller(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all students (API v2 – includes age)")]
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

        var v2Items = result.Items.Select(s => new StudentV2ResponseDto
        {
            StudentId = s.StudentId,
            FullName = s.FullName,
            Email = s.Email,
            DateOfBirth = s.DateOfBirth,
            Age = CalculateAge(s.DateOfBirth),
            ApiVersion = "2.0"
        }).ToList();

        var v2Result = PagedResult<StudentV2ResponseDto>.Create(
            v2Items, result.Pagination.TotalItems, result.Pagination.Page, result.Pagination.PageSize);

        return Ok(ListApiHelper.ToListResponse(v2Result, "Students retrieved successfully.", fields));
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }
}
