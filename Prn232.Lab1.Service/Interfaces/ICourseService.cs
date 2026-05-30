using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ICourseService
{
    Task<Pagination<CourseResponseDto>> GetCoursesAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<CourseResponseDto> GetCourseByDetailAsync(int id);
    Task<CourseResponseDto> GetEnrollmentByCourseAsync(int id);
    Task<CourseResponseDto> CreateCourseAsync(CourseCreateRequestDto request);
    Task<CourseResponseDto> UpdateCourseAsync(int id, CourseUpdateRequestDto request);
    Task DeleteCourseAsync(int id);
}
