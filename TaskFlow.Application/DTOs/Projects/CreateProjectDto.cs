namespace TaskFlow.Application.DTOs.Projects;

public record CreateProjectDto(
    string Name,
    string? Description);