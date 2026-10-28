using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task AddAsync(Project project);
    void Update(Project project);
    void Delete(Project project);
    Task SaveChangesAsync();
    Task<List<Project>> GetByOwnerIdAsync(int ownerId);
}