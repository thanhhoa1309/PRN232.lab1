using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Courses;

public static class CourseListDto
{
    public static CourseResponse FromEntity(Course entity, string? expand = null)
    {
        var response = new CourseResponse
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId
        };

        if (!string.IsNullOrWhiteSpace(expand))
        {
            if (expand.Contains("semester", StringComparison.OrdinalIgnoreCase) && entity.Semester != null)
            {
                response.Semester = SemesterSummaryResponse.FromEntity(entity.Semester);
            }

            if (expand.Contains("enrollments", StringComparison.OrdinalIgnoreCase))
            {
                response.Enrollments = entity.Enrollments?
                    .Select(EnrollmentSummaryResponse.FromEntity)
                    .ToList();
            }
        }

        return response;
    }
}
