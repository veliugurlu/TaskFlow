using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks;

public class TaskQueryParameters
{
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public bool Desc { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}