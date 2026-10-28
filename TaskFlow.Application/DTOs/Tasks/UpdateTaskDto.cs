using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks;

public record UpdateTaskDto(
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    int? AssigneeId);