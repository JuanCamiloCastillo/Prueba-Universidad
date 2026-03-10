using FluentValidation;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public class EnrollStudentValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0);
    }
}
