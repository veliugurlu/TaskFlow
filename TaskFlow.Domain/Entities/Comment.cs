using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
}
