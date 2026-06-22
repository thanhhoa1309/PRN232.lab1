using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentResponseDto>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand);

    Task<StudentResponseDto> GetStudentByIdAsync(int id);
    Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto request);
    Task<StudentResponseDto> UpdateStudentAsync(int id, StudentUpdateRequestDto request);
    Task DeleteStudentAsync(int id);
}
