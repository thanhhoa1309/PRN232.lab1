using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Subjects;

public static class SubjectCreateDto
{
    public static Subject ToEntity(SubjectCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SubjectCode) || string.IsNullOrWhiteSpace(request.SubjectName))
        {
            throw ErrorHelper.BadRequest("SubjectCode and SubjectName are required.");
        }

        return new Subject
        {
            SubjectCode = request.SubjectCode.Trim(),
            SubjectName = request.SubjectName.Trim(),
            Credit = request.Credit
        };
    }

    public static SubjectResponse FromEntity(Subject entity) => SubjectListDto.FromEntity(entity);
}
