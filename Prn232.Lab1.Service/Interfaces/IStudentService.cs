using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IStudentService
{
    Task<Pagination<StudentResponseDto>> GetStudentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand);

    Task<StudentResponseDto> GetStudentByIdAsync(int id);
    Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto request);
    Task<StudentResponseDto> UpdateStudentAsync(int id, StudentUpdateRequestDto request);
    Task DeleteStudentAsync(int id);
}
