using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.Common;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;

    public ProjectsController(
        IProjectService service,
        IValidator<CreateProjectDto> createValidator,
        IValidator<UpdateProjectDto> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        if (CurrentUserId is not int userId) return Unauthorized();
        return Ok(await _service.GetAllAsync(userId, IsAdmin));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        if (CurrentUserId is not int userId) return Unauthorized();

        var project = await _service.GetByIdAsync(id, userId, IsAdmin);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        if (CurrentUserId is not int ownerId) return Unauthorized();

        var created = await _service.CreateAsync(dto, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        if (CurrentUserId is not int userId) return Unauthorized();

        var result = await _service.UpdateAsync(id, dto, userId, IsAdmin);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (CurrentUserId is not int userId) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId, IsAdmin);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult(OperationResult result) => result switch
    {
        OperationResult.Success => NoContent(),
        OperationResult.Forbidden => Forbid(),
        _ => NotFound()
    };
}