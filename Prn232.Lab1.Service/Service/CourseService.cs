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

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CourseResponseDto>> GetCoursesAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var includeSemester = ExpandHelper.HasExpand(expand, "semester");

        IQueryable<Course> query = _unitOfWork.CourseRepository.GetAllAsQueryable();

        if (includeSemester)
        {
            query = query.Include(c => c.Semester);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(c => c.CourseName.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Course, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["courseId"] = c => c.CourseId,
            ["courseName"] = c => c.CourseName,
            ["semesterId"] = c => c.SemesterId
        };

        query = QueryHelper.ApplySorting(query, sort, sortMap);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var responses = items
            .Select(c => MapToResponse(c, includeSemester))
            .ToList();

        return PagedResult<CourseResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<CourseResponseDto> GetCourseByDetailAsync(int id)
    {
        var entity = await _unitOfWork.CourseRepository.GetAllAsQueryable()
            .Include(c => c.Semester)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        return MapToResponse(entity, includeSemester: true);
    }

    public async Task<CourseResponseDto> GetEnrollmentByCourseAsync(int id)
    {
        var entity = await _unitOfWork.CourseRepository.GetAllAsQueryable()
            .Include(c => c.Semester)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        return new CourseResponseDto
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId,
            Semester = entity.Semester != null
                ? new SemesterResponseDto
                {
                    SemesterId = entity.Semester.SemesterId,
                    SemesterName = entity.Semester.SemesterName,
                    StartDate = entity.Semester.StartDate,
                    EndDate = entity.Semester.EndDate
                }
                : null,
            Enrollments = entity.Enrollments.Select(e => new EnrollmentResponseDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status
            }).ToList(),
            Students = entity.Enrollments
                .Where(e => e.Student != null)
                .Select(e => new StudentResponseDto
                {
                    StudentId = e.Student!.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                })
                .ToList()
        };
    }

    public async Task<PagedResult<StudentResponseDto>> GetEnrolledStudentsByCourseAsync(
        int courseId,
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        if (!await _unitOfWork.CourseRepository.GetAllAsQueryable().AnyAsync(c => c.CourseId == courseId))
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        var studentIds = _unitOfWork.EnrollmentRepository.GetAllAsQueryable()
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .Distinct();

        IQueryable<Student> query = _unitOfWork.StudentRepository.GetAllAsQueryable()
            .Where(s => studentIds.Contains(s.StudentId));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(s => s.FullName.Contains(keyword) || s.Email.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Student, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["studentId"] = s => s.StudentId,
            ["fullName"] = s => s.FullName,
            ["email"] = s => s.Email,
            ["dateOfBirth"] = s => s.DateOfBirth
        };

        query = QueryHelper.ApplySorting(query, sort, sortMap);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items.Select(s => new StudentResponseDto
        {
            StudentId = s.StudentId,
            FullName = s.FullName,
            Email = s.Email,
            DateOfBirth = s.DateOfBirth
        }).ToList();

        return PagedResult<StudentResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<PagedResult<EnrollmentResponseDto>> GetEnrollmentsByCourseAsync(
        int courseId,
        string? status,
        string? sort,
        int page,
        int pageSize,
        string? fields)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        if (!await _unitOfWork.CourseRepository.GetAllAsQueryable().AnyAsync(c => c.CourseId == courseId))
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        IQueryable<Enrollment> query = _unitOfWork.EnrollmentRepository.GetAllAsQueryable()
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusFilter = status.Trim();
            query = query.Where(e => e.Status.Contains(statusFilter));
        }

        var sortMap = new Dictionary<string, Expression<Func<Enrollment, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["enrollmentId"] = e => e.EnrollmentId,
            ["studentId"] = e => e.StudentId,
            ["courseId"] = e => e.CourseId,
            ["enrollDate"] = e => e.EnrollDate,
            ["status"] = e => e.Status
        };

        query = QueryHelper.ApplySorting(query, sort ?? "-enrollDate", sortMap);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items.Select(e => new EnrollmentResponseDto
        {
            EnrollmentId = e.EnrollmentId,
            StudentId = e.StudentId,
            CourseId = e.CourseId,
            EnrollDate = e.EnrollDate,
            Status = e.Status,
            Student = e.Student == null ? null : new StudentResponseDto
            {
                StudentId = e.Student.StudentId,
                FullName = e.Student.FullName,
                Email = e.Student.Email,
                DateOfBirth = e.Student.DateOfBirth
            }
        }).ToList();

        return PagedResult<EnrollmentResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<CourseResponseDto> CreateCourseAsync(CourseCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CourseName))
        {
            throw ErrorHelper.BadRequest("CourseName is required.");
        }

        var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(request.SemesterId);
        if (semester == null)
        {
            throw ErrorHelper.BadRequest("SemesterId does not exist.");
        }

        var entity = new Course
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId
        };

        await _unitOfWork.CourseRepository.CreateAsync(entity);

        return MapToResponse(entity, includeSemester: false);
    }

    public async Task<CourseResponseDto> UpdateCourseAsync(int id, CourseUpdateRequestDto request)
    {
        var existing = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        if (request.SemesterId.HasValue)
        {
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(request.SemesterId.Value);
            if (semester == null)
            {
                throw ErrorHelper.BadRequest("SemesterId does not exist.");
            }
        }

        UpdateHelper.ApplyUpdates(existing, request);
        await _unitOfWork.CourseRepository.UpdateAsync(existing);

        return MapToResponse(existing, includeSemester: false);
    }

    public async Task DeleteCourseAsync(int id)
    {
        var existing = await _unitOfWork.CourseRepository.GetAllAsQueryable()
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (existing == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        if (existing.Enrollments.Any())
        {
            throw ErrorHelper.BadRequest("Cannot delete course that has enrollments.");
        }

        await _unitOfWork.CourseRepository.RemoveAsync(existing);
    }

    private static CourseResponseDto MapToResponse(Course entity, bool includeSemester)
    {
        return new CourseResponseDto
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId,
            Semester = includeSemester && entity.Semester != null
                ? new SemesterResponseDto
                {
                    SemesterId = entity.Semester.SemesterId,
                    SemesterName = entity.Semester.SemesterName,
                    StartDate = entity.Semester.StartDate,
                    EndDate = entity.Semester.EndDate
                }
                : null
        };
    }
}
