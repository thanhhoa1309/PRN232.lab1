using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Enrollments;
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

    public async Task<Pagination<EnrollmentResponse>> GetEnrollmentsAsync(
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
        var responses = items.Select(e => EnrollmentListDto.FromEntity(e, expand)).ToList();

        return new Pagination<EnrollmentResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<EnrollmentResponse> GetEnrollmentByIdAsync(int id)
    {
        var entity = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        return EnrollmentListDto.FromEntity(entity);
    }

    public async Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentCreateRequest request)
    {
        var entity = await EnrollmentCreateDto.ToEntityAsync(request, _unitOfWork);
        await _unitOfWork.EnrollmentRepository.CreateAsync(entity);
        return EnrollmentCreateDto.FromEntity(entity);
    }

    public async Task<EnrollmentResponse> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequest request)
    {
        var existing = await _unitOfWork.EnrollmentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Enrollment not found.");
        }

        await EnrollmentUpdateDto.ApplyAsync(existing, request, _unitOfWork);
        await _unitOfWork.EnrollmentRepository.UpdateAsync(existing);
        return EnrollmentUpdateDto.FromEntity(existing);
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
