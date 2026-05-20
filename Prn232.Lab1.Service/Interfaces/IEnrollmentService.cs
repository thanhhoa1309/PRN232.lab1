using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IEnrollmentService
{
    Task<Pagination<EnrollmentResponse>> GetEnrollmentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<EnrollmentResponse> GetEnrollmentByIdAsync(int id);
    Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentCreateRequest request);
    Task<EnrollmentResponse> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequest request);
    Task DeleteEnrollmentAsync(int id);
}
