using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Students;

public static class StudentUpdateDto
{
    public static void Apply(Student entity, StudentUpdateRequest request)
    {
        UpdateHelper.ApplyUpdates(entity, request);
    }

    public static StudentResponse FromEntity(Student entity) => StudentListDto.FromEntity(entity);
}
