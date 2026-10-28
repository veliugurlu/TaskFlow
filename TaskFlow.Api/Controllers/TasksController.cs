using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    private readonly IValidator<CreateTaskDto> _createValidator;
    private readonly IValidator<UpdateTaskDto> _updateValidator;

    public TasksController(
        ITaskService service,
        IValidator<CreateTaskDto> createValidator,
        IValidator<UpdateTaskDto> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetByProject(
        int projectId, [FromQuery] TaskQueryParameters query)
    {
        if (CurrentUserId is not int userId) return Unauthorized();

        var result = await _service.GetPagedByProjectIdAsync(projectId, query, userId, IsAdmin);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        if (CurrentUserId is not int userId) return Unauthorized();

        var task = await _service.GetByIdAsync(id, userId, IsAdmin);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        if (CurrentUserId is not int userId) return Unauthorized();

        var (result, created) = await _service.CreateAsync(dto, userId, IsAdmin);

        return result switch
        {
            OperationResult.Success => CreatedAtAction(nameof(GetById), new { id = created!.Id }, created),
            OperationResult.Forbidden => Forbid(),
            _ => NotFound(new { error = "Proje bulunamadı." })
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        if (CurrentUserId is not int userId) return Unauthorized();

        return ToActionResult(await _service.UpdateAsync(id, dto, userId, IsAdmin));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (CurrentUserId is not int userId) return Unauthorized();

        return ToActionResult(await _service.DeleteAsync(id, userId, IsAdmin));
    }

    private IActionResult ToActionResult(OperationResult result) => result switch
    {
        OperationResult.Success => NoContent(),
        OperationResult.Forbidden => Forbid(),
        _ => NotFound()
    };
}