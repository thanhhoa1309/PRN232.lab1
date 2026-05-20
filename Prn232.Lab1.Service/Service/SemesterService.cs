using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
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

    public async Task<Pagination<SemesterResponse>> GetSemestersAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        IQueryable<Semester> dbQuery = _unitOfWork.SemesterRepository.GetAllAsQueryable();

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

        dbQuery = QueryHelper.ApplySorting(dbQuery, sortBy, isDescending, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var responses = items.Select(s => SemesterListDto.FromEntity(s, expand)).ToList();

        return new Pagination<SemesterResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<SemesterResponse> GetSemesterByIdAsync(int id)
    {
        var entity = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        return SemesterListDto.FromEntity(entity);
    }

    public async Task<SemesterResponse> CreateSemesterAsync(SemesterCreateRequest request)
    {
        var entity = SemesterCreateDto.ToEntity(request);
        await _unitOfWork.SemesterRepository.CreateAsync(entity);
        return SemesterCreateDto.FromEntity(entity);
    }

    public async Task<SemesterResponse> UpdateSemesterAsync(int id, SemesterUpdateRequest request)
    {
        var existing = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        SemesterUpdateDto.Apply(existing, request);
        await _unitOfWork.SemesterRepository.UpdateAsync(existing);
        return SemesterUpdateDto.FromEntity(existing);
    }

    public async Task DeleteSemesterAsync(int id)
    {
        var existing = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Semester not found.");
        }

        await _unitOfWork.SemesterRepository.RemoveAsync(existing);
    }
}
