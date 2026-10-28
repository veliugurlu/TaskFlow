using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks;

public record CreateTaskDto(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int ProjectId,
    int? AssigneeId);