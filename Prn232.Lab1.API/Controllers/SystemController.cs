using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Prn232.Lab1.API.Controllers;

[Route("api/system")]
[ApiController]
public class SystemController : ControllerBase
{
    private const int MinSemesters = 5;
    private const int MinSubjects = 10;
    private const int MinStudents = 50;
    private const int MinCourses = 20;
    private const int MinEnrollments = 500;

    private readonly Prn232Lab1DbContext _dbContext;

    public SystemController(Prn232Lab1DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================================================================
    // SEED DATA  —  POST /api/system/seed
    // =========================================================================

    [HttpPost("seed")]
    [SwaggerOperation(
        Summary = "Seed sample data",
        Description = "Populates the database with sample semesters, subjects, students, courses, and enrollments.")]
    [ProducesResponseType(typeof(ApiResult<object>), 200)]
    public async Task<IActionResult> SeedData()
    {
        if (await _dbContext.Semesters.AnyAsync()
            || await _dbContext.Subjects.AnyAsync()
            || await _dbContext.Students.AnyAsync()
            || await _dbContext.Courses.AnyAsync()
            || await _dbContext.Enrollments.AnyAsync())
        {
            return Ok(ApiResult<object>.Success(new { message = "Data already seeded." }, "200", "Data already seeded."));
        }

        var random = new Random(2026);

        var semesters = Enumerable.Range(1, MinSemesters)
            .Select(i => new Semester
            {
                SemesterName = $"Semester {i}",
                StartDate = new DateTime(2024, 1, 1).AddMonths((i - 1) * 4),
                EndDate = new DateTime(2024, 4, 30).AddMonths((i - 1) * 4)
            })
            .ToList();

        var subjects = Enumerable.Range(1, MinSubjects)
            .Select(i => new Subject
            {
                SubjectCode = $"SUB{i:000}",
                SubjectName = $"Subject {i}",
                Credit = random.Next(1, 5)
            })
            .ToList();

        var students = Enumerable.Range(1, MinStudents)
            .Select(i => new Student
            {
                FullName = $"Student {i}",
                Email = $"student{i}@lms.local",
                DateOfBirth = new DateTime(2003, 1, 1).AddDays(random.Next(0, 365 * 5))
            })
            .ToList();

        var courses = Enumerable.Range(1, MinCourses)
            .Select(i => new Course
            {
                CourseName = $"{subjects[(i - 1) % subjects.Count].SubjectCode} - Class {(i - 1) / subjects.Count + 1}",
                Semester = semesters[(i - 1) % semesters.Count]
            })
            .ToList();

        var enrollments = new List<Enrollment>();
        for (var i = 1; i <= MinEnrollments; i++)
        {
            enrollments.Add(new Enrollment
            {
                Student = students[random.Next(students.Count)],
                Course = courses[random.Next(courses.Count)],
                EnrollDate = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                Status = random.Next(0, 2) == 0 ? "Active" : "Completed"
            });
        }

        await _dbContext.Semesters.AddRangeAsync(semesters);
        await _dbContext.Subjects.AddRangeAsync(subjects);
        await _dbContext.Students.AddRangeAsync(students);
        await _dbContext.Courses.AddRangeAsync(courses);
        await _dbContext.Enrollments.AddRangeAsync(enrollments);
        await _dbContext.SaveChangesAsync();

        var summary = new
        {
            message = "Seed data created.",
            semesters = semesters.Count,
            subjects = subjects.Count,
            students = students.Count,
            courses = courses.Count,
            enrollments = enrollments.Count
        };

        return Ok(ApiResult<object>.Success(summary, "200", "Seed data created."));
    }

    // =========================================================================
    // CLEAR DATA  —  DELETE /api/system/clear
    // =========================================================================

    [HttpDelete("clear")]
    [SwaggerOperation(
        Summary = "Clear all data",
        Description = "Removes all semesters, subjects, students, courses, and enrollments from the database.")]
    [ProducesResponseType(typeof(ApiResult<object>), 200)]
    public async Task<IActionResult> ClearData()
    {
        _dbContext.Enrollments.RemoveRange(_dbContext.Enrollments);
        _dbContext.Courses.RemoveRange(_dbContext.Courses);
        _dbContext.Students.RemoveRange(_dbContext.Students);
        _dbContext.Subjects.RemoveRange(_dbContext.Subjects);
        _dbContext.Semesters.RemoveRange(_dbContext.Semesters);

        await _dbContext.SaveChangesAsync();

        return Ok(ApiResult<object>.Success(new { message = "Data cleared." }, "200", "Data cleared."));
    }
}
