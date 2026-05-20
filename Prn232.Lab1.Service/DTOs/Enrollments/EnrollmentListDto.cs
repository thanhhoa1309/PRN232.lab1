using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public static class EnrollmentListDto
{
    public static EnrollmentResponse FromEntity(Enrollment entity, string? expand = null)
    {
        var response = new EnrollmentResponse
        {
            EnrollmentId = entity.EnrollmentId,
            StudentId = entity.StudentId,
            CourseId = entity.CourseId,
            EnrollDate = entity.EnrollDate,
            Status = entity.Status
        };

        if (!string.IsNullOrWhiteSpace(expand))
        {
            if (expand.Contains("student", StringComparison.OrdinalIgnoreCase) && entity.Student != null)
            {
                response.Student = StudentSummaryResponse.FromEntity(entity.Student);
            }

            if (expand.Contains("course", StringComparison.OrdinalIgnoreCase) && entity.Course != null)
            {
                response.Course = CourseSummaryResponse.FromEntity(entity.Course, includeSemester: true);
            }
        }

        return response;
    }
}
