using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Students;

public static class StudentListDto
{
    public static StudentResponse FromEntity(Student entity, string? expand = null)
    {
        var response = new StudentResponse
        {
            StudentId = entity.StudentId,
            FullName = entity.FullName,
            Email = entity.Email,
            DateOfBirth = entity.DateOfBirth
        };

        if (!string.IsNullOrWhiteSpace(expand) && expand.Contains("enrollments", StringComparison.OrdinalIgnoreCase))
        {
            response.Enrollments = entity.Enrollments?
                .Select(EnrollmentSummaryResponse.FromEntity)
                .ToList();
        }

        return response;
    }
}
