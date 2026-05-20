using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Service;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<StudentResponse>> GetStudentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        IQueryable<Student> dbQuery = _unitOfWork.StudentRepository.GetAllAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(s => s.FullName.Contains(keyword) || s.Email.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Student, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["studentId"] = s => s.StudentId,
            ["fullName"] = s => s.FullName,
            ["email"] = s => s.Email,
            ["dateOfBirth"] = s => s.DateOfBirth
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sortBy, isDescending, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var responses = items.Select(s => StudentListDto.FromEntity(s, expand)).ToList();

        return new Pagination<StudentResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<StudentResponse> GetStudentByIdAsync(int id)
    {
        var entity = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        return StudentListDto.FromEntity(entity);
    }

    public async Task<StudentResponse> CreateStudentAsync(StudentCreateRequest request)
    {
        var entity = StudentCreateDto.ToEntity(request);
        await _unitOfWork.StudentRepository.CreateAsync(entity);
        return StudentCreateDto.FromEntity(entity);
    }

    public async Task<StudentResponse> UpdateStudentAsync(int id, StudentUpdateRequest request)
    {
        var existing = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        StudentUpdateDto.Apply(existing, request);
        await _unitOfWork.StudentRepository.UpdateAsync(existing);
        return StudentUpdateDto.FromEntity(existing);
    }

    public async Task DeleteStudentAsync(int id)
    {
        var existing = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        await _unitOfWork.StudentRepository.RemoveAsync(existing);
    }
}
