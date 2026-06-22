using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Dtos.Subjects;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Service;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SubjectResponseDto>> GetSubjectsAsync(
        string? search,
        string? sort,
        int page,
        int pageSize,
        string? fields,
        string? expand)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        IQueryable<Subject> dbQuery = _unitOfWork.SubjectRepository.GetAllAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            dbQuery = dbQuery.Where(s => s.SubjectName.Contains(keyword) || s.SubjectCode.Contains(keyword));
        }

        var sortMap = new Dictionary<string, Expression<Func<Subject, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["subjectId"] = s => s.SubjectId,
            ["subjectCode"] = s => s.SubjectCode,
            ["subjectName"] = s => s.SubjectName,
            ["credit"] = s => s.Credit
        };

        dbQuery = QueryHelper.ApplySorting(dbQuery, sort, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var responses = items.Select(MapToResponse).ToList();

        return PagedResult<SubjectResponseDto>.Create(responses, totalCount, page, pageSize);
    }

    public async Task<SubjectResponseDto> GetSubjectByIdAsync(int id)
    {
        var entity = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Subject not found.");
        }

        return MapToResponse(entity);
    }

    public async Task<SubjectResponseDto> CreateSubjectAsync(SubjectCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SubjectCode) || string.IsNullOrWhiteSpace(request.SubjectName))
        {
            throw ErrorHelper.BadRequest("SubjectCode and SubjectName are required.");
        }

        var entity = new Subject
        {
            SubjectCode = request.SubjectCode.Trim(),
            SubjectName = request.SubjectName.Trim(),
            Credit = request.Credit
        };

        await _unitOfWork.SubjectRepository.CreateAsync(entity);

        return MapToResponse(entity);
    }

    public async Task<SubjectResponseDto> UpdateSubjectAsync(int id, SubjectUpdateRequestDto request)
    {
        var existing = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Subject not found.");
        }

        UpdateHelper.ApplyUpdates(existing, request);
        await _unitOfWork.SubjectRepository.UpdateAsync(existing);

        return MapToResponse(existing);
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var existing = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Subject not found.");
        }

        await _unitOfWork.SubjectRepository.RemoveAsync(existing);
    }

    private static SubjectResponseDto MapToResponse(Subject entity)
    {
        return new SubjectResponseDto
        {
            SubjectId = entity.SubjectId,
            SubjectCode = entity.SubjectCode,
            SubjectName = entity.SubjectName,
            Credit = entity.Credit
        };
    }
}
