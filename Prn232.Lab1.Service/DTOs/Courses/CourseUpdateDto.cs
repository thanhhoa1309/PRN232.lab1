using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Dtos.Courses;

public static class CourseUpdateDto
{
    public static async Task ApplyAsync(Course entity, CourseUpdateRequest request, IUnitOfWork unitOfWork)
    {
        if (request.SemesterId.HasValue)
        {
            var semester = await unitOfWork.SemesterRepository.GetByIdAsync(request.SemesterId.Value);
            if (semester == null)
            {
                throw ErrorHelper.BadRequest("SemesterId does not exist.");
            }
        }

        UpdateHelper.ApplyUpdates(entity, request);
    }

    public static CourseResponse FromEntity(Course entity) => CourseListDto.FromEntity(entity);
}
