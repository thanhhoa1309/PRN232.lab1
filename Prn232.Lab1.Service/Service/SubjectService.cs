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

    public async Task<Pagination<SubjectResponse>> GetSubjectsAsync(
        string? search,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
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

        dbQuery = QueryHelper.ApplySorting(dbQuery, sortBy, isDescending, sortMap);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var responses = items.Select(SubjectListDto.FromEntity).ToList();

        return new Pagination<SubjectResponse>(responses, totalCount, page, pageSize);
    }

    public async Task<SubjectResponse> GetSubjectByIdAsync(int id)
    {
        var entity = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw ErrorHelper.NotFound("Subject not found.");
        }

        return SubjectListDto.FromEntity(entity);
    }

    public async Task<SubjectResponse> CreateSubjectAsync(SubjectCreateRequest request)
    {
        var entity = SubjectCreateDto.ToEntity(request);
        await _unitOfWork.SubjectRepository.CreateAsync(entity);
        return SubjectCreateDto.FromEntity(entity);
    }

    public async Task<SubjectResponse> UpdateSubjectAsync(int id, SubjectUpdateRequest request)
    {
        var existing = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw ErrorHelper.NotFound("Subject not found.");
        }

        SubjectUpdateDto.Apply(existing, request);
        await _unitOfWork.SubjectRepository.UpdateAsync(existing);
        return SubjectUpdateDto.FromEntity(existing);
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
}
