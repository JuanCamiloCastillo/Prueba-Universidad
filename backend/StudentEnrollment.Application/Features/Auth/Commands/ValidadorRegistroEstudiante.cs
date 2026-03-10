using FluentValidation;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class ValidadorRegistroEstudiante : AbstractValidator<ComandoRegistroEstudiante>
{
    public ValidadorRegistroEstudiante()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Contrasena).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}
