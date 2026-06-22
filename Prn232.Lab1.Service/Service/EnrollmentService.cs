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

    public async Task<PagedResult<EnrollmentResponseDto>> GetEnrollmentsAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var includeStudent = ExpandHelper.HasExpand(expand, "student");
        var includeCourse = ExpandHelper.HasExpand(expand, "course");

        IQueryable<Enrollment> dbQuery = _unitOfWork.EnrollmentRepository.GetAllAsQueryable();

        if (includeStudent)
        {
            dbQuery = dbQuery.Include(e => e.Student);
        }

        if (includeCourse)
        {
            dbQuery = dbQuery.Include(e => e.Course).ThenInclude(c => c!.Semester);
        }

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

        dbQuery = QueryHelper.ApplySorting(dbQuery, sort, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items
            .Select(e => MapToResponse(e, includeStudent, includeCourse))
            .ToList();

        return PagedResult<EnrollmentResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<EnrollmentResponseDto> GetEnrollmentByIdAsync(int id)
    {
        var entity = await _unitOfWork.EnrollmentRepository.GetAllAsQueryable()
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c!.Semester)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        return MapToResponse(entity, includeStudent: true, includeCourse: true);
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

        return MapToResponse(entity, includeStudent: false, includeCourse: false);
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

        return MapToResponse(existing, includeStudent: false, includeCourse: false);
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

    private static EnrollmentResponseDto MapToResponse(Enrollment entity, bool includeStudent, bool includeCourse)
    {
        return new EnrollmentResponseDto
        {
            EnrollmentId = entity.EnrollmentId,
            StudentId = entity.StudentId,
            CourseId = entity.CourseId,
            EnrollDate = entity.EnrollDate,
            Status = entity.Status,
            Student = includeStudent && entity.Student != null
                ? new StudentResponseDto
                {
                    StudentId = entity.Student.StudentId,
                    FullName = entity.Student.FullName,
                    Email = entity.Student.Email,
                    DateOfBirth = entity.Student.DateOfBirth
                }
                : null,
            Course = includeCourse && entity.Course != null
                ? new CourseResponseDto
                {
                    CourseId = entity.Course.CourseId,
                    CourseName = entity.Course.CourseName,
                    SemesterId = entity.Course.SemesterId,
                    Semester = entity.Course.Semester != null
                        ? new SemesterResponseDto
                        {
                            SemesterId = entity.Course.Semester.SemesterId,
                            SemesterName = entity.Course.Semester.SemesterName,
                            StartDate = entity.Course.Semester.StartDate,
                            EndDate = entity.Course.Semester.EndDate
                        }
                        : null
                }
                : null
        };
    }
}
