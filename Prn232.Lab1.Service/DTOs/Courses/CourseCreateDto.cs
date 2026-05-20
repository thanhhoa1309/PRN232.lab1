using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Courses;

public static class CourseCreateDto
{
    public static async Task<Course> ToEntityAsync(CourseCreateRequest request, IUnitOfWork unitOfWork)
    {
        if (string.IsNullOrWhiteSpace(request.CourseName))
        {
            throw ErrorHelper.BadRequest("CourseName is required.");
        }

        var semester = await unitOfWork.SemesterRepository.GetByIdAsync(request.SemesterId);
        if (semester == null)
        {
            throw ErrorHelper.BadRequest("SemesterId does not exist.");
        }

        return new Course
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId
        };
    }

    public static CourseResponse FromEntity(Course entity) => CourseListDto.FromEntity(entity);
}
