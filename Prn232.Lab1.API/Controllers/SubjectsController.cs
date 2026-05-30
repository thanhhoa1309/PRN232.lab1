using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/subjects")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // =========================================================================
    // GET ALL  —  GET /api/subjects
    // =========================================================================

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all subjects",
        Description = "Retrieve a paginated list of subjects with optional search and sort options.")]
    [ProducesResponseType(typeof(ApiResult<Pagination<SubjectResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<IActionResult> GetAllSubjects(
        [FromQuery, SwaggerParameter(Description = "Search by subject name or code (optional)")] string? search = null,
        [FromQuery, SwaggerParameter(Description = "Sort by field: subjectId, subjectCode, subjectName, credit (optional)")] string? sortBy = null,
        [FromQuery, SwaggerParameter(Description = "Sort in descending order? Default: false")] bool isDescending = false,
        [FromQuery, SwaggerParameter(Description = "Page number, starting from 1")] int page = 1,
        [FromQuery, SwaggerParameter(Description = "Number of items per page")] int pageSize = 10,
        [FromQuery, SwaggerParameter(Description = "Expand related data (optional)")] string? expand = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Invalid pagination parameters."));
        }

        var result = await _subjectService.GetSubjectsAsync(search, sortBy, isDescending, page, pageSize, expand);

        return Ok(ApiResult<Pagination<SubjectResponseDto>>.Success(result, "200", "Subjects retrieved successfully."));
    }

    // =========================================================================
    // GET BY ID  —  GET /api/subjects/{id}
    // =========================================================================

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get subject details",
        Description = "Retrieve detailed information for a specific subject by ID.")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> GetSubjectById([FromRoute] int id)
    {
        var result = await _subjectService.GetSubjectByIdAsync(id);

        return Ok(ApiResult<SubjectResponseDto>.Success(result, "200", "Subject retrieved successfully."));
    }

    // =========================================================================
    // CREATE  —  POST /api/subjects    // =========================================================================

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new subject",
        Description = "Creates a new subject with the provided information.")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> CreateSubject(
        [FromBody, SwaggerParameter("New subject data to be created")] SubjectCreateRequestDto request)
    {
        var result = await _subjectService.CreateSubjectAsync(request);

        return CreatedAtAction(
            nameof(GetSubjectById),
            new { id = result.SubjectId },
            ApiResult<SubjectResponseDto>.Success(result, "201", "Subject created successfully."));
    }

    // =========================================================================
    // UPDATE  —  PUT /api/subjects/{id}         // =========================================================================

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update subject information",
        Description = "Updates the details of a specific subject by ID.")]
    [ProducesResponseType(typeof(ApiResult<SubjectResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 409)]
    public async Task<IActionResult> UpdateSubject(
        [FromRoute] int id,
        [FromBody, SwaggerParameter("Updated subject data")] SubjectUpdateRequestDto request)
    {
        if (request == null)
        {
            return BadRequest(ApiResult<object>.Failure("400", "Subject update data is required."));
        }

        var result = await _subjectService.UpdateSubjectAsync(id, request);

        return Ok(ApiResult<SubjectResponseDto>.Success(result, "200", "Subject updated successfully."));
    }

    // =========================================================================
    // DELETE  —  DELETE /api/subjects/{id}      // =========================================================================

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a subject",
        Description = "Deletes a subject by ID.")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<IActionResult> DeleteSubject([FromRoute] int id)
    {
        await _subjectService.DeleteSubjectAsync(id);

        return Ok(ApiResult<bool>.Success(true, "200", "Subject deleted successfully."));
    }
}
