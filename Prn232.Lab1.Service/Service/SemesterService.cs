using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Courses;
using Prn232.Lab1.Service.Dtos.Semesters;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Service;

public class SemesterService : ISemesterService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SemesterResponseDto>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var includeCourses = ExpandHelper.HasExpand(expand, "courses");

        IQueryable<Semester> dbQuery = _unitOfWork.SemesterRepository.GetAllAsQueryable();

        if (includeCourses)
        {
            dbQuery = dbQuery.Include(s => s.Courses);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(s => s.SemesterName.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Semester, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["semesterId"] = s => s.SemesterId,
            ["semesterName"] = s => s.SemesterName,
            ["startDate"] = s => s.StartDate,
            ["endDate"] = s => s.EndDate
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sort, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items
            .Select(s => MapToResponse(s, includeCourses))
            .ToList();

        return PagedResult<SemesterResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<SemesterResponseDto> GetSemesterByIdAsync(int id)
    {
        var entity = await _unitOfWork.SemesterRepository.GetAllAsQueryable()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id);

        if (entity == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        return MapToResponse(entity, includeCourses: true);
    }

    public async Task<SemesterResponseDto> CreateSemesterAsync(SemesterCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SemesterName))
        {
            throw ErrorHelper.BadRequest("SemesterName is required.");
        }

        ResourceHelper.DateTimeValidate(request.StartDate, request.EndDate);

        var entity = new Semester
        {
            SemesterName = request.SemesterName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        await _unitOfWork.SemesterRepository.CreateAsync(entity);

        return MapToResponse(entity, includeCourses: false);
    }

    public async Task<SemesterResponseDto> UpdateSemesterAsync(int id, SemesterUpdateRequestDto request)
    {
        var existing = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        UpdateHelper.ApplyUpdates(existing, request);

        if (existing.StartDate != default && existing.EndDate != default)
        {
            ResourceHelper.DateTimeValidate(existing.StartDate, existing.EndDate);
        }

        await _unitOfWork.SemesterRepository.UpdateAsync(existing);

        return MapToResponse(existing, includeCourses: false);
    }

    public async Task DeleteSemesterAsync(int id)
    {
        var existing = await _unitOfWork.SemesterRepository.GetAllAsQueryable()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id);

        if (existing == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        if (existing.Courses.Any())
        {
            throw ErrorHelper.BadRequest("Cannot delete semester that has courses.");
        }

        await _unitOfWork.SemesterRepository.RemoveAsync(existing);
    }

    private static SemesterResponseDto MapToResponse(Semester entity, bool includeCourses)
    {
        return new SemesterResponseDto
        {
            SemesterId = entity.SemesterId,
            SemesterName = entity.SemesterName,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Courses = includeCourses
                ? entity.Courses?.Select(c => new CourseResponseDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                }).ToList()
                : null
        };
    }
}
