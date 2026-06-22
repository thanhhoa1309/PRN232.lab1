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

    public async Task<PagedResult<StudentResponseDto>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var includeEnrollments = ExpandHelper.HasExpand(expand, "enrollments");

        IQueryable<Student> dbQuery = _unitOfWork.StudentRepository.GetAllAsQueryable();

        if (includeEnrollments)
        {
            dbQuery = dbQuery.Include(s => s.Enrollments);
        }

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

        dbQuery = QueryHelper.ApplySorting(dbQuery, sort, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items
            .Select(s => MapToResponse(s, includeEnrollments))
            .ToList();

        return PagedResult<StudentResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<StudentResponseDto> GetStudentByIdAsync(int id)
    {
        var entity = await _unitOfWork.StudentRepository.GetAllAsQueryable()
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        return MapToResponse(entity, includeEnrollments: true);
    }

    public async Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email))
        {
            throw ErrorHelper.BadRequest("FullName and Email are required.");
        }

        var email = request.Email.Trim();
        if (await EmailExistsAsync(email))
        {
            throw ErrorHelper.BadRequest("Email already exists.");
        }

        var entity = new Student
        {
            FullName = request.FullName.Trim(),
            Email = email,
            DateOfBirth = request.DateOfBirth
        };

        await _unitOfWork.StudentRepository.CreateAsync(entity);

        return MapToResponse(entity, includeEnrollments: false);
    }

    public async Task<StudentResponseDto> UpdateStudentAsync(int id, StudentUpdateRequestDto request)
    {
        var existing = await _unitOfWork.StudentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            if (await EmailExistsAsync(email, id))
            {
                throw ErrorHelper.BadRequest("Email already exists.");
            }
        }

        UpdateHelper.ApplyUpdates(existing, request);
        await _unitOfWork.StudentRepository.UpdateAsync(existing);

        return MapToResponse(existing, includeEnrollments: false);
    }

    public async Task DeleteStudentAsync(int id)
    {
        var existing = await _unitOfWork.StudentRepository.GetAllAsQueryable()
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (existing == null)
        {
            throw ErrorHelper.NotFound("Student not found.");
        }

        if (existing.Enrollments.Any())
        {
            throw ErrorHelper.BadRequest("Cannot delete student that has enrollments.");
        }

        await _unitOfWork.StudentRepository.RemoveAsync(existing);
    }

    private async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _unitOfWork.StudentRepository.GetAllAsQueryable()
            .Where(s => s.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.StudentId != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    private static StudentResponseDto MapToResponse(Student entity, bool includeEnrollments)
    {
        return new StudentResponseDto
        {
            StudentId = entity.StudentId,
            FullName = entity.FullName,
            Email = entity.Email,
            DateOfBirth = entity.DateOfBirth,
            Enrollments = includeEnrollments
                ? entity.Enrollments?.Select(e => new EnrollmentResponseDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList()
                : null
        };
    }
}
