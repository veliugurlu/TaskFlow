using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Comment> Comments => Set<Comment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) // 
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);
            e.Property(x => x.Description).HasMaxLength(1000);

            e.HasOne(x => x.Owner)
             .WithMany(u => u.OwnedProjects)
             .HasForeignKey(x => x.OwnerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(2000);

            e.HasOne(x => x.Project)
             .WithMany(p => p.Tasks)
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Assignee)
             .WithMany(u => u.AssignedTasks)
             .HasForeignKey(x => x.AssigneeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectMember>(e =>
        {
            e.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();

            e.HasOne(x => x.Project)
             .WithMany(p => p.Members)
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.User)
             .WithMany(u => u.Memberships)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.Property(x => x.Content).IsRequired().HasMaxLength(2000);

            e.HasOne(x => x.TaskItem)
             .WithMany(t => t.Comments)
             .HasForeignKey(x => x.TaskItemId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Author)
             .WithMany(u => u.Comments)
             .HasForeignKey(x => x.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}