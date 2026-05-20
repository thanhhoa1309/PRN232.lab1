using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISemesterService
{
    Task<Pagination<SemesterResponse>> GetSemestersAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize);

    Task<SemesterResponse> GetSemesterByIdAsync(int id, string? expand);
    Task<SemesterResponse> CreateSemesterAsync(SemesterCreateRequest request);
    Task<SemesterResponse> UpdateSemesterAsync(int id, SemesterUpdateRequest request);
    Task DeleteSemesterAsync(int id);
}
