using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ICourseService
{
    Task<Pagination<CourseResponse>> GetCoursesAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize);

    Task<CourseResponse> GetCourseByIdAsync(int id, string? expand);
    Task<CourseResponse> CreateCourseAsync(CourseCreateRequest request);
    Task<CourseResponse> UpdateCourseAsync(int id, CourseUpdateRequest request);
    Task DeleteCourseAsync(int id);
}
