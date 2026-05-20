using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Subjects;

public static class SubjectUpdateDto
{
    public static void Apply(Subject entity, SubjectUpdateRequest request)
    {
        UpdateHelper.ApplyUpdates(entity, request);
    }

    public static SubjectResponse FromEntity(Subject entity) => SubjectListDto.FromEntity(entity);
}
