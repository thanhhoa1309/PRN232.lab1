using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISemesterService
{
    Task<Pagination<SemesterResponseDto>> GetSemestersAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<SemesterResponseDto> GetSemesterByIdAsync(int id);
    Task<SemesterResponseDto> CreateSemesterAsync(SemesterCreateRequestDto request);
    Task<SemesterResponseDto> UpdateSemesterAsync(int id, SemesterUpdateRequestDto request);
    Task DeleteSemesterAsync(int id);
}
