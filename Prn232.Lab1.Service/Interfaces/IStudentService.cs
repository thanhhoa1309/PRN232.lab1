using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Interfaces;

public interface IStudentService
{
    Task<Pagination<StudentResponse>> GetStudentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize);

    Task<StudentResponse> GetStudentByIdAsync(int id, string? expand);
    Task<StudentResponse> CreateStudentAsync(StudentCreateRequest request);
    Task<StudentResponse> UpdateStudentAsync(int id, StudentUpdateRequest request);
    Task DeleteStudentAsync(int id);
}
