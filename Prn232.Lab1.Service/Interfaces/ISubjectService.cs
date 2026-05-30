using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISubjectService
{
    Task<Pagination<SubjectResponseDto>> GetSubjectsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<SubjectResponseDto> GetSubjectByIdAsync(int id);
    Task<SubjectResponseDto> CreateSubjectAsync(SubjectCreateRequestDto request);
    Task<SubjectResponseDto> UpdateSubjectAsync(int id, SubjectUpdateRequestDto request);
    Task DeleteSubjectAsync(int id);
}
