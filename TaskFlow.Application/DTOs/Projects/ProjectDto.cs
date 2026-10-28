namespace TaskFlow.Application.DTOs.Projects;

public record ProjectDto(
    int Id,
    string Name,
    string? Description,
    int OwnerId,
    DateTime CreatedAt);