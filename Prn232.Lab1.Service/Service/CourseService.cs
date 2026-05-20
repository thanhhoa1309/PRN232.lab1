using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Courses;
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

        IQueryable<Course> dbQuery = _unitOfWork.CourseRepository.GetAllAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(c => c.CourseName.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Course, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["courseId"] = c => c.CourseId,
            ["courseName"] = c => c.CourseName,
            ["semesterId"] = c => c.SemesterId
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sortBy, isDescending, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var responses = items.Select(c => CourseListDto.FromEntity(c, expand)).ToList();

        return new Pagination<CourseResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<CourseResponse> GetCourseByIdAsync(int id)
    {
        var entity = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        return CourseListDto.FromEntity(entity);
    }

    public async Task<CourseResponse> CreateCourseAsync(CourseCreateRequest request)
    {
        var entity = await CourseCreateDto.ToEntityAsync(request, _unitOfWork);
        await _unitOfWork.CourseRepository.CreateAsync(entity);
        return CourseCreateDto.FromEntity(entity);
    }

    public async Task<CourseResponse> UpdateCourseAsync(int id, CourseUpdateRequest request)
    {
        var existing = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Course not found.");
        }

        await CourseUpdateDto.ApplyAsync(existing, request, _unitOfWork);
        await _unitOfWork.CourseRepository.UpdateAsync(existing);
        return CourseUpdateDto.FromEntity(existing);
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
