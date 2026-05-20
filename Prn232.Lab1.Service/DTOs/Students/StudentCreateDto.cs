using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Students;

public static class StudentCreateDto
{
    public static Student ToEntity(StudentCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email))
        {
            throw ErrorHelper.BadRequest("FullName and Email are required.");
        }

        return new Student
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            DateOfBirth = request.DateOfBirth
        };
    }

    public static StudentResponse FromEntity(Student entity) => StudentListDto.FromEntity(entity);
}
