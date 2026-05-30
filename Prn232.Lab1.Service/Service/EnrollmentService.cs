using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Dtos.Enrollments;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Dtos.Students;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Service;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<EnrollmentResponseDto>> GetEnrollmentsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        IQueryable<Enrollment> dbQuery = _unitOfWork.EnrollmentRepository.GetAllAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(e => e.Status.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Enrollment, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["enrollmentId"] = e => e.EnrollmentId,
            ["studentId"] = e => e.StudentId,
            ["courseId"] = e => e.CourseId,
            ["enrollDate"] = e => e.EnrollDate,
            ["status"] = e => e.Status
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sortBy, isDescending, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var includeStudent = !string.IsNullOrWhiteSpace(expand)
            && expand.Contains("student", StringComparison.OrdinalIgnoreCase);
        var includeCourse = !string.IsNullOrWhiteSpace(expand)
            && expand.Contains("course", StringComparison.OrdinalIgnoreCase);

        var responses = items.Select(e => new EnrollmentResponseDto
        {
            EnrollmentId = e.EnrollmentId,
            StudentId = e.StudentId,
            CourseId = e.CourseId,
            EnrollDate = e.EnrollDate,
            Status = e.Status,
            Student = includeStudent && e.Student != null
                ? new StudentResponseDto
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                }
                : null,
            Course = includeCourse && e.Course != null
                ? new CourseResponseDto
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId,
                    Semester = e.Course.Semester != null
                        ? new SemesterResponseDto
                        {
                            SemesterId = e.Course.Semester.SemesterId,
                            SemesterName = e.Course.Semester.SemesterName,
                            StartDate = e.Course.Semester.StartDate,
                            EndDate = e.Course.Semester.EndDate
                        }
                        : null
                }
                : null
        }).ToList();

        return new Pagination<EnrollmentResponseDto>(responses, totalCount, page, pageSize);
    }

    public async Task<EnrollmentResponseDto> GetEnrollmentByIdAsync(int id)
    {
        var entity = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        return new EnrollmentResponseDto
        {
            EnrollmentId = entity.EnrollmentId,
            StudentId = entity.StudentId,
            CourseId = entity.CourseId,
            EnrollDate = entity.EnrollDate,
            Status = entity.Status
        };
    }

    public async Task<EnrollmentResponseDto> CreateEnrollmentAsync(EnrollmentCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw ErrorHelper.BadRequest("Status is required.");
        }

        var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId);
        if (student == null)
        {
            throw ErrorHelper.BadRequest("StudentId does not exist.");
        }

        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            throw ErrorHelper.BadRequest("CourseId does not exist.");
        }

        var entity = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate ?? DateTime.UtcNow,
            Status = request.Status.Trim()
        };

        await _unitOfWork.EnrollmentRepository.CreateAsync(entity);

        return new EnrollmentResponseDto
        {
            EnrollmentId = entity.EnrollmentId,
            StudentId = entity.StudentId,
            CourseId = entity.CourseId,
            EnrollDate = entity.EnrollDate,
            Status = entity.Status
        };
    }

    public async Task<EnrollmentResponseDto> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequestDto request)
    {
        var existing = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        if (request.StudentId.HasValue)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId.Value);
            if (student == null)
            {
                throw ErrorHelper.BadRequest("StudentId does not exist.");
            }
        }

        if (request.CourseId.HasValue)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId.Value);
            if (course == null)
            {
                throw ErrorHelper.BadRequest("CourseId does not exist.");
            }
        }

        UpdateHelper.ApplyUpdates(existing, request);
        await _unitOfWork.EnrollmentRepository.UpdateAsync(existing);

        return new EnrollmentResponseDto
        {
            EnrollmentId = existing.EnrollmentId,
            StudentId = existing.StudentId,
            CourseId = existing.CourseId,
            EnrollDate = existing.EnrollDate,
            Status = existing.Status
        };
    }

    public async Task DeleteEnrollmentAsync(int id)
    {
        var existing = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        await _unitOfWork.EnrollmentRepository.RemoveAsync(existing);
    }
}
