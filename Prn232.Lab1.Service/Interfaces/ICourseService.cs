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
        int pageSize,
        string? expand);

    Task<CourseResponse> GetCourseByDetailAsync(int id);
    Task<CourseResponse> GetEnrollmentByCourseAsync(int id);
    Task<CourseResponse> CreateCourseAsync(CourseCreateRequest request);
    Task<CourseResponse> UpdateCourseAsync(int id, CourseUpdateRequest request);
    Task DeleteCourseAsync(int id);
}
