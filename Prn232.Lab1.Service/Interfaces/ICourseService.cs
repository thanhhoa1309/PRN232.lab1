using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseResponseDto>> GetCoursesAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand);

    Task<CourseResponseDto> GetCourseByDetailAsync(int id);
    Task<CourseResponseDto> GetEnrollmentByCourseAsync(int id);
    Task<PagedResult<StudentResponseDto>> GetEnrolledStudentsByCourseAsync(
        int courseId,
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields);
    Task<PagedResult<EnrollmentResponseDto>> GetEnrollmentsByCourseAsync(
        int courseId,
        string? status,
        string? sort,
        int page,
        int pageSize,
        string? fields);
    Task<CourseResponseDto> CreateCourseAsync(CourseCreateRequestDto request);
    Task<CourseResponseDto> UpdateCourseAsync(int id, CourseUpdateRequestDto request);
    Task DeleteCourseAsync(int id);
}
