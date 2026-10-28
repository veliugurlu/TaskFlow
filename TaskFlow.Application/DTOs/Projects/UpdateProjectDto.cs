namespace TaskFlow.Application.DTOs.Projects;

public record UpdateProjectDto(
    string Name,
    string? Description);