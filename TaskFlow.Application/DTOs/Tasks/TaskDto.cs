using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks;

public record TaskDto(
    int Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    int ProjectId,
    int? AssigneeId,
    DateTime CreatedAt);