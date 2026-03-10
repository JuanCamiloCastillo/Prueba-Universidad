using FluentValidation;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public class ValidadorInscripcion : AbstractValidator<ComandoInscripcion>
{
    public ValidadorInscripcion()
    {
        RuleFor(x => x.IdAsignatura).GreaterThan(0);
    }
}
