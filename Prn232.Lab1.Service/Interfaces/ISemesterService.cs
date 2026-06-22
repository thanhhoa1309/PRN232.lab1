using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISemesterService
{
    Task<PagedResult<SemesterResponseDto>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand);

    Task<SemesterResponseDto> GetSemesterByIdAsync(int id);
    Task<SemesterResponseDto> CreateSemesterAsync(SemesterCreateRequestDto request);
    Task<SemesterResponseDto> UpdateSemesterAsync(int id, SemesterUpdateRequestDto request);
    Task DeleteSemesterAsync(int id);
}
