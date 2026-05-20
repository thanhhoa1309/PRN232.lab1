using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface ISubjectService
{
    Task<Pagination<SubjectResponse>> GetSubjectsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize);

    Task<SubjectResponse> GetSubjectByIdAsync(int id);
    Task<SubjectResponse> CreateSubjectAsync(SubjectCreateRequest request);
    Task<SubjectResponse> UpdateSubjectAsync(int id, SubjectUpdateRequest request);
    Task DeleteSubjectAsync(int id);
}
