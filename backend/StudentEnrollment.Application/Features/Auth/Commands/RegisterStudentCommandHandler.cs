using MediatR;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, int>
{
    private readonly IStudentRepository _studentRepository;

    public RegisterStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<int> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        var existing = await _studentRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered.");

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _studentRepository.AddAsync(student, cancellationToken);
        await _studentRepository.SaveChangesAsync(cancellationToken);
        return student.Id;
    }
}
