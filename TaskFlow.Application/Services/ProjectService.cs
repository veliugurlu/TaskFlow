using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProjectDto>> GetAllAsync(int userId, bool isAdmin)
    {
        var projects = isAdmin
            ? await _repository.GetAllAsync()
            : await _repository.GetByOwnerIdAsync(userId);

        return projects.Select(ToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, int userId, bool isAdmin)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project is null || !CanAccess(project, userId, isAdmin))
            return null;

        return ToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, int ownerId)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = ownerId
        };

        await _repository.AddAsync(project);
        await _repository.SaveChangesAsync();

        return ToDto(project);
    }

    public async Task<OperationResult> UpdateAsync(int id, UpdateProjectDto dto, int userId, bool isAdmin)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project is null) return OperationResult.NotFound;
        if (!CanAccess(project, userId, isAdmin)) return OperationResult.Forbidden;

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.UpdatedAt = DateTime.UtcNow;

        _repository.Update(project);
        await _repository.SaveChangesAsync();
        return OperationResult.Success;
    }

    public async Task<OperationResult> DeleteAsync(int id, int userId, bool isAdmin)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project is null) return OperationResult.NotFound;
        if (!CanAccess(project, userId, isAdmin)) return OperationResult.Forbidden;

        _repository.Delete(project);
        await _repository.SaveChangesAsync();
        return OperationResult.Success;
    }

    private static bool CanAccess(Project project, int userId, bool isAdmin) =>
        isAdmin || project.OwnerId == userId;

    private static ProjectDto ToDto(Project p) =>
        new(p.Id, p.Name, p.Description, p.OwnerId, p.CreatedAt);
}