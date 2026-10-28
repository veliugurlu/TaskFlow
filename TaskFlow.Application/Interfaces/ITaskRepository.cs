using System.Net.NetworkInformation;
using TaskFlow.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;
using TaskFlow.Application.DTOs.Tasks;
namespace TaskFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetByProjectIdAsync(int projectId);
    Task<TaskItem?> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    void Update(TaskItem task);
    void Delete(TaskItem task);
    Task<bool> ProjectExistsAsync(int projectId);
    Task SaveChangesAsync();
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedByProjectIdAsync(
    int projectId, TaskQueryParameters query);
    Task<int?> GetProjectOwnerIdAsync(int projectId);
}