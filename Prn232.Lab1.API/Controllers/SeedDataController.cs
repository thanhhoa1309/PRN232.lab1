using Microsoft.AspNetCore.Mvc;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[ApiController]
[Route("api/seed-data")]
[SwaggerTag("Seed Data")]
public class SeedDataController : ControllerBase
{
    private readonly ISeedDataService _seedDataService;

    public SeedDataController(ISeedDataService seedDataService)
    {
        _seedDataService = seedDataService;
    }

    [HttpPost("seed")]
    [SwaggerOperation(
        Summary = "Seed sample data",
        Description = "Clears existing data and populates semesters, subjects, students, courses, and enrollments.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    public async Task<IActionResult> SeedData()
    {
        var summary = await _seedDataService.SeedAsync();
        return Ok(ApiResult.SuccessResult(summary, "Seed data created."));
    }

    [HttpDelete("clear")]
    [SwaggerOperation(
        Summary = "Clear all LMS data",
        Description = "Removes all semesters, subjects, students, courses, and enrollments from the database.")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    public async Task<IActionResult> ClearData()
    {
        await _seedDataService.ClearAsync();
        return Ok(ApiResult.SuccessResult(new { message = "Data cleared." }, "Data cleared."));
    }
}
