using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Enrollments;
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

    public async Task<Pagination<StudentResponseDto>> GetStudentsAsync(
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

        var includeEnrollments = !string.IsNullOrWhiteSpace(expand)
            && expand.Contains("enrollments", StringComparison.OrdinalIgnoreCase);

        var responses = items.Select(s => new StudentResponseDto
        {
            StudentId = s.StudentId,
            FullName = s.FullName,
            Email = s.Email,
            DateOfBirth = s.DateOfBirth,
            Enrollments = includeEnrollments
                ? s.Enrollments?.Select(e => new EnrollmentResponseDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList()
                : null
        }).ToList();

        return new Pagination<StudentResponseDto>(responses, totalCount, page, pageSize);
    }

    public async Task<StudentResponseDto> GetStudentByIdAsync(int id)
    {
        var entity = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        return new StudentResponseDto
        {
            StudentId = entity.StudentId,
            FullName = entity.FullName,
            Email = entity.Email,
            DateOfBirth = entity.DateOfBirth
        };
    }

    public async Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email))
        {
            throw ErrorHelper.BadRequest("FullName and Email are required.");
        }

        var entity = new Student
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            DateOfBirth = request.DateOfBirth
        };

        await _unitOfWork.StudentRepository.CreateAsync(entity);

        return new StudentResponseDto
        {
            StudentId = entity.StudentId,
            FullName = entity.FullName,
            Email = entity.Email,
            DateOfBirth = entity.DateOfBirth
        };
    }

    public async Task<StudentResponseDto> UpdateStudentAsync(int id, StudentUpdateRequestDto request)
    {
        var existing = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        UpdateHelper.ApplyUpdates(existing, request);
        await _unitOfWork.StudentRepository.UpdateAsync(existing);

        return new StudentResponseDto
        {
            StudentId = existing.StudentId,
            FullName = existing.FullName,
            Email = existing.Email,
            DateOfBirth = existing.DateOfBirth
        };
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
