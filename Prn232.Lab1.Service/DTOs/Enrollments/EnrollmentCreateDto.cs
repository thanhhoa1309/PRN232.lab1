using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Enrollments;

public static class EnrollmentCreateDto
{
    public static async Task<Enrollment> ToEntityAsync(EnrollmentCreateRequest request, IUnitOfWork unitOfWork)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw ErrorHelper.BadRequest("Status is required.");
        }

        var student = await unitOfWork.StudentRepository.GetByIdAsync(request.StudentId);
        if (student == null)
        {
            throw ErrorHelper.BadRequest("StudentId does not exist.");
        }

        var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            throw ErrorHelper.BadRequest("CourseId does not exist.");
        }

        return new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate ?? DateTime.UtcNow,
            Status = request.Status.Trim()
        };
    }

    public static EnrollmentResponse FromEntity(Enrollment entity) => EnrollmentListDto.FromEntity(entity);
}
