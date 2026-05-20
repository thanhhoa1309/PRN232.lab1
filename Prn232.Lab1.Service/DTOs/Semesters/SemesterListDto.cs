using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Dtos;

namespace Prn232.Lab1.Service.Dtos.Semesters;

public static class SemesterListDto
{
    public static SemesterResponse FromEntity(Semester entity, string? expand = null)
    {
        var response = new SemesterResponse
        {
            SemesterId = entity.SemesterId,
            SemesterName = entity.SemesterName,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };

        if (!string.IsNullOrWhiteSpace(expand) && expand.Contains("courses", StringComparison.OrdinalIgnoreCase))
        {
            response.Courses = entity.Courses?
                .Select(c => CourseSummaryResponse.FromEntity(c))
                .ToList();
        }

        return response;
    }
}
