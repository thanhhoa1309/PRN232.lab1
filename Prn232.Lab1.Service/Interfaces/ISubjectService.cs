using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISubjectService
{
    Task<PagedResult<SubjectResponseDto>> GetSubjectsAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand);

    Task<SubjectResponseDto> GetSubjectByIdAsync(int id);
    Task<SubjectResponseDto> CreateSubjectAsync(SubjectCreateRequestDto request);
    Task<SubjectResponseDto> UpdateSubjectAsync(int id, SubjectUpdateRequestDto request);
    Task DeleteSubjectAsync(int id);
}
