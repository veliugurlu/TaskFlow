using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;
namespace TaskFlow.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public void Update(TaskItem task)
    {
        _context.Tasks.Update(task);
    }

    public void Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task<bool> ProjectExistsAsync(int projectId)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedByProjectIdAsync(
    int projectId, TaskQueryParameters query)
    {
        var q = _context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (query.Status.HasValue)
            q = q.Where(t => t.Status == query.Status.Value);

        if (query.Priority.HasValue)
            q = q.Where(t => t.Priority == query.Priority.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(t => t.Title.Contains(query.Search));

        var totalCount = await q.CountAsync();

        q = (query.SortBy.ToLowerInvariant(), query.Desc) switch
        {
            ("priority", true) => q.OrderByDescending(t => t.Priority),
            ("priority", false) => q.OrderBy(t => t.Priority),
            ("duedate", true) => q.OrderByDescending(t => t.DueDate),
            ("duedate", false) => q.OrderBy(t => t.DueDate),
            (_, true) => q.OrderByDescending(t => t.CreatedAt),
            _ => q.OrderBy(t => t.CreatedAt)
        };

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<int?> GetProjectOwnerIdAsync(int projectId)
    {
        return await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => (int?)p.OwnerId)
            .FirstOrDefaultAsync();
    }
}