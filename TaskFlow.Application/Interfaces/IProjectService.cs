using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Projects;

namespace TaskFlow.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync(int userId, bool isAdmin);
    Task<ProjectDto?> GetByIdAsync(int id, int userId, bool isAdmin);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, int ownerId);
    Task<OperationResult> UpdateAsync(int id, UpdateProjectDto dto, int userId, bool isAdmin);
    Task<OperationResult> DeleteAsync(int id, int userId, bool isAdmin);
}