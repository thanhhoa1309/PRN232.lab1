using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Service;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<CourseResponse>> GetCoursesAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _unitOfWork.CourseRepository.GetAllAsQueryable();

        var includeSemester = !string.IsNullOrWhiteSpace(expand)
            && expand.Contains("semester", StringComparison.OrdinalIgnoreCase);

        if (includeSemester)
        {
            query = query.Include(c => c.Semester);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(c => c.CourseName.Contains(keyword));
        }

        query = sortBy?.ToLower() switch
        {
            "courseid" => isDescending ? query.OrderByDescending(c => c.CourseId) : query.OrderBy(c => c.CourseId),
            "coursename" => isDescending ? query.OrderByDescending(c => c.CourseName) : query.OrderBy(c => c.CourseName),
            "semesterid" => isDescending ? query.OrderByDescending(c => c.SemesterId) : query.OrderBy(c => c.SemesterId),
            _ => isDescending ? query.OrderByDescending(c => c.CourseId) : query.OrderBy(c => c.CourseId),
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var responses = items.Select(c => new CourseResponse
        {
            CourseId = c.CourseId,
            CourseName = c.CourseName,
            SemesterId = c.SemesterId,
            Semester = includeSemester && c.Semester != null
                ? new SemesterSummaryResponse
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                }
                : null
        }).ToList();

        return new Pagination<CourseResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<CourseResponse> GetCourseByDetailAsync(int id)
    {
        var entity = await _unitOfWork.CourseRepository.GetAllAsQueryable()
            .Include(c => c.Semester)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        return new CourseResponse
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId,
            Semester = entity.Semester != null
                ? new SemesterSummaryResponse
                {
                    SemesterId = entity.Semester.SemesterId,
                    SemesterName = entity.Semester.SemesterName,
                    StartDate = entity.Semester.StartDate,
                    EndDate = entity.Semester.EndDate
                }
                : null
        };
    }

    public async Task<CourseResponse> GetEnrollmentByCourseAsync(int id)
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

        return new CourseResponse
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId,
            Semester = entity.Semester != null
                ? new SemesterSummaryResponse
                {
                    SemesterId = entity.Semester.SemesterId,
                    SemesterName = entity.Semester.SemesterName,
                    StartDate = entity.Semester.StartDate,
                    EndDate = entity.Semester.EndDate
                }
                : null,
            Enrollments = entity.Enrollments.Select(e => new EnrollmentSummaryResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status
            }).ToList(),
            Students = entity.Enrollments
                .Where(e => e.Student != null)
                .Select(e => new StudentSummaryResponse
                {
                    StudentId = e.Student!.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                })
                .ToList()
        };
    }

    public async Task<CourseResponse> CreateCourseAsync(CourseCreateRequest request)
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

        return new CourseResponse
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId
        };
    }

    public async Task<CourseResponse> UpdateCourseAsync(int id, CourseUpdateRequest request)
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

        return new CourseResponse
        {
            CourseId = existing.CourseId,
            CourseName = existing.CourseName,
            SemesterId = existing.SemesterId
        };
    }

    public async Task DeleteCourseAsync(int id)
    {
        var existing = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        await _unitOfWork.CourseRepository.RemoveAsync(existing);
    }
}
