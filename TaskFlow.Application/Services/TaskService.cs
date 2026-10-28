using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TaskDto>?> GetPagedByProjectIdAsync(
        int projectId, TaskQueryParameters query, int userId, bool isAdmin)
    {
        if (await CheckProjectAccessAsync(projectId, userId, isAdmin) != OperationResult.Success)
            return null;

        query.Page = Math.Max(query.Page, 1);
        query.PageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, totalCount) = await _repository.GetPagedByProjectIdAsync(projectId, query);

        return new PagedResult<TaskDto>(
            items.Select(ToDto).ToList(),
            query.Page,
            query.PageSize,
            totalCount);
    }

    public async Task<TaskDto?> GetByIdAsync(int id, int userId, bool isAdmin)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null) return null;

        if (await CheckProjectAccessAsync(task.ProjectId, userId, isAdmin) != OperationResult.Success)
            return null;

        return ToDto(task);
    }

    public async Task<(OperationResult Result, TaskDto? Task)> CreateAsync(
        CreateTaskDto dto, int userId, bool isAdmin)
    {
        var access = await CheckProjectAccessAsync(dto.ProjectId, userId, isAdmin);
        if (access != OperationResult.Success)
            return (access, null);

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            AssigneeId = dto.AssigneeId
        };

        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        return (OperationResult.Success, ToDto(task));
    }

    public async Task<OperationResult> UpdateAsync(int id, UpdateTaskDto dto, int userId, bool isAdmin)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null) return OperationResult.NotFound;

        var access = await CheckProjectAccessAsync(task.ProjectId, userId, isAdmin);
        if (access != OperationResult.Success) return access;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.AssigneeId = dto.AssigneeId;
        task.UpdatedAt = DateTime.UtcNow;

        _repository.Update(task);
        await _repository.SaveChangesAsync();
        return OperationResult.Success;
    }

    public async Task<OperationResult> DeleteAsync(int id, int userId, bool isAdmin)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null) return OperationResult.NotFound;

        var access = await CheckProjectAccessAsync(task.ProjectId, userId, isAdmin);
        if (access != OperationResult.Success) return access;

        _repository.Delete(task);
        await _repository.SaveChangesAsync();
        return OperationResult.Success;
    }

    private async Task<OperationResult> CheckProjectAccessAsync(int projectId, int userId, bool isAdmin)
    {
        var ownerId = await _repository.GetProjectOwnerIdAsync(projectId);

        if (ownerId is null) return OperationResult.NotFound;
        if (!isAdmin && ownerId.Value != userId) return OperationResult.Forbidden;

        return OperationResult.Success;
    }

    private static TaskDto ToDto(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.Status, t.Priority,
            t.DueDate, t.ProjectId, t.AssigneeId, t.CreatedAt);
}