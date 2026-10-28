using FluentValidation;

namespace TaskFlow.Application.DTOs.Projects;

public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Proje adı boş olamaz.")
            .MaximumLength(150).WithMessage("Proje adı en fazla 150 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");
    }
}