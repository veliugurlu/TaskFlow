using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Tasks;

namespace TaskFlow.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResult<TaskDto>?> GetPagedByProjectIdAsync(
        int projectId, TaskQueryParameters query, int userId, bool isAdmin);
    Task<TaskDto?> GetByIdAsync(int id, int userId, bool isAdmin);
    Task<(OperationResult Result, TaskDto? Task)> CreateAsync(
        CreateTaskDto dto, int userId, bool isAdmin);
    Task<OperationResult> UpdateAsync(int id, UpdateTaskDto dto, int userId, bool isAdmin);
    Task<OperationResult> DeleteAsync(int id, int userId, bool isAdmin);
}