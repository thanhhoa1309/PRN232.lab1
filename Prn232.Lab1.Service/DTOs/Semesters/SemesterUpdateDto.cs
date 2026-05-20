using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Semesters;

public static class SemesterUpdateDto
{
    public static void Apply(Semester entity, SemesterUpdateRequest request)
    {
        UpdateHelper.ApplyUpdates(entity, request);

        if (entity.StartDate != default && entity.EndDate != default)
        {
            ResourceHelper.DateTimeValidate(entity.StartDate, entity.EndDate);
        }
    }

    public static SemesterResponse FromEntity(Semester entity) => SemesterListDto.FromEntity(entity);
}
