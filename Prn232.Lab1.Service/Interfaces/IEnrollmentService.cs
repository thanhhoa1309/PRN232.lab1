using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IEnrollmentService
{
    Task<Pagination<EnrollmentResponseDto>> GetEnrollmentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<EnrollmentResponseDto> GetEnrollmentByIdAsync(int id);
    Task<EnrollmentResponseDto> CreateEnrollmentAsync(EnrollmentCreateRequestDto request);
    Task<EnrollmentResponseDto> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequestDto request);
    Task DeleteEnrollmentAsync(int id);
}
