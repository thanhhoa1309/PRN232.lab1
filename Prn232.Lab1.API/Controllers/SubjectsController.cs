using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/subjects")]
[Authorize]
[SwaggerTag("Subjects")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all subjects",
        Description = "Retrieve a paginated list of subjects with optional search, sort, and fields options.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    public async Task<IActionResult> GetAllSubjects(
        [FromQuery, SwaggerParameter(Description = "Search by subject name or code")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort fields, e.g. subjectCode,-credit")] string? sort = null,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int? size = null,
        [FromQuery, SwaggerParameter(Description = "Alias of size")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Select fields, e.g. subjectId,subjectCode")] string? fields = null,
        [FromQuery, SwaggerParameter(Description = "Expand related data")] string? expand = null)
    {
        var resolvedPageSize = ListApiHelper.ResolvePageSize(size, pageSize);
        if (page < 1 || resolvedPageSize < 1)
        {
            return BadRequest(ApiResult.FailureResult("Invalid pagination parameters."));
        }

        var result = await _subjectService.GetSubjectsAsync(search, sort, page, resolvedPageSize, fields, expand);
        return Ok(ListApiHelper.ToListResponse(result, "Subjects retrieved successfully.", fields));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get subject details")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> GetSubjectById([FromRoute] int id)
    {
        var result = await _subjectService.GetSubjectByIdAsync(id);
        return Ok(ApiResult<SubjectResponseDto>.Ok(result, "Subject retrieved successfully."));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new subject")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> CreateSubject(
        [FromBody, SwaggerParameter("New subject data to be created")] SubjectCreateRequestDto request)
    {
        var result = await _subjectService.CreateSubjectAsync(request);

        return CreatedAtAction(
            nameof(GetSubjectById),
            new { id = result.SubjectId },
            ApiResult<SubjectResponseDto>.Ok(result, "Subject created successfully."));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update subject information")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult), 400)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    [ProducesResponseType(typeof(ApiResult), 409)]
    public async Task<IActionResult> UpdateSubject(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated subject data")] SubjectUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult.FailureResult("Subject update data is required."));
        }

        var result = await _subjectService.UpdateSubjectAsync(id, request);
        return Ok(ApiResult<SubjectResponseDto>.Ok(result, "Subject updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a subject")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult), 404)]
    public async Task<IActionResult> DeleteSubject([FromRoute] int id)
    {
        await _subjectService.DeleteSubjectAsync(id);
        return Ok(ApiResult<bool>.Ok(true, "Subject deleted successfully."));
    }
}
