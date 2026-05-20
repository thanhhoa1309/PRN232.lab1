using Prn232.Lab1.Repositories.Domain;

namespace Prn232.Lab1.Service.Dtos.Subjects;

public static class SubjectListDto
{
    public static SubjectResponse FromEntity(Subject entity) => new()
    {
        SubjectId = entity.SubjectId,
        SubjectCode = entity.SubjectCode,
        SubjectName = entity.SubjectName,
        Credit = entity.Credit
    };
}
