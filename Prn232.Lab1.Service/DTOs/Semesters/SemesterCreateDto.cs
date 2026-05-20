using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Semesters;

public static class SemesterCreateDto
{
    public static Semester ToEntity(SemesterCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SemesterName))
        {
            throw ErrorHelper.BadRequest("SemesterName is required.");
        }

        ResourceHelper.DateTimeValidate(request.StartDate, request.EndDate);

        return new Semester
        {
            SemesterName = request.SemesterName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }

    public static SemesterResponse FromEntity(Semester entity) => SemesterListDto.FromEntity(entity);
}
