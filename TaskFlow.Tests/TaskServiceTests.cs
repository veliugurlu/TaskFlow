using Moq;
using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repo = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _service = new TaskService(_repo.Object);
    }

    private static CreateTaskDto NewDto(int projectId) =>
        new("Başlık", null, TaskPriority.Medium, null, projectId, null);

    [Fact]
    public async Task CreateAsync_ProjectDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _repo.Setup(r => r.GetProjectOwnerIdAsync(99)).ReturnsAsync((int?)null);

        // Act
        var (result, task) = await _service.CreateAsync(NewDto(99), userId: 1, isAdmin: false);

        // Assert
        Assert.Equal(OperationResult.NotFound, result);
        Assert.Null(task);
        _repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserIsNotOwner_ReturnsForbidden()
    {
        _repo.Setup(r => r.GetProjectOwnerIdAsync(10)).ReturnsAsync(5);

        var (result, task) = await _service.CreateAsync(NewDto(10), userId: 1, isAdmin: false);

        Assert.Equal(OperationResult.Forbidden, result);
        Assert.Null(task);
        _repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserIsOwner_CreatesTask()
    {
        _repo.Setup(r => r.GetProjectOwnerIdAsync(10)).ReturnsAsync(1);

        var (result, task) = await _service.CreateAsync(NewDto(10), userId: 1, isAdmin: false);

        Assert.Equal(OperationResult.Success, result);
        Assert.NotNull(task);
        _repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_AdminIsNotOwner_StillCreatesTask()
    {
        _repo.Setup(r => r.GetProjectOwnerIdAsync(10)).ReturnsAsync(5);

        var (result, _) = await _service.CreateAsync(NewDto(10), userId: 1, isAdmin: true);

        Assert.Equal(OperationResult.Success, result);
    }
}