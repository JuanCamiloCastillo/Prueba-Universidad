using FluentValidation;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class ValidadorInicioSesion : AbstractValidator<ComandoInicioSesion>
{
    public ValidadorInicioSesion()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Contrasena).NotEmpty();
    }
}
