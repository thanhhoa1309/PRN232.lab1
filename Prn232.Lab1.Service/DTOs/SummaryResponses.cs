using Prn232.Lab1.Repositories.Domain;

namespace Prn232.Lab1.Service.Dtos;

public class SemesterSummaryResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public static SemesterSummaryResponse FromEntity(Semester entity) => new()
    {
        SemesterId = entity.SemesterId,
        SemesterName = entity.SemesterName,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate
    };
}

public class CourseSummaryResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public SemesterSummaryResponse? Semester { get; set; }

    public static CourseSummaryResponse FromEntity(Course entity, bool includeSemester = false) => new()
    {
        CourseId = entity.CourseId,
        CourseName = entity.CourseName,
        SemesterId = entity.SemesterId,
        Semester = includeSemester && entity.Semester != null
            ? SemesterSummaryResponse.FromEntity(entity.Semester)
            : null
    };
}

public class StudentSummaryResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public static StudentSummaryResponse FromEntity(Student entity) => new()
    {
        StudentId = entity.StudentId,
        FullName = entity.FullName,
        Email = entity.Email,
        DateOfBirth = entity.DateOfBirth
    };
}

public class EnrollmentSummaryResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public static EnrollmentSummaryResponse FromEntity(Enrollment entity) => new()
    {
        EnrollmentId = entity.EnrollmentId,
        StudentId = entity.StudentId,
        CourseId = entity.CourseId,
        EnrollDate = entity.EnrollDate,
        Status = entity.Status
    };
}
