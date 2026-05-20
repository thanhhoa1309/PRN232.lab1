using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public static class EnrollmentUpdateDto
{
    public static async Task ApplyAsync(Enrollment entity, EnrollmentUpdateRequest request, IUnitOfWork unitOfWork)
    {
        if (request.StudentId.HasValue)
        {
            var student = await unitOfWork.StudentRepository.GetByIdAsync(request.StudentId.Value);
            if (student == null)
            {
                throw ErrorHelper.BadRequest("StudentId does not exist.");
            }
        }

        if (request.CourseId.HasValue)
        {
            var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId.Value);
            if (course == null)
            {
                throw ErrorHelper.BadRequest("CourseId does not exist.");
            }
        }

        UpdateHelper.ApplyUpdates(entity, request);
    }

    public static EnrollmentResponse FromEntity(Enrollment entity) => EnrollmentListDto.FromEntity(entity);
}
