using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IStudentRepository studentRepository, IJwtService jwtService)
    {
        _studentRepository = studentRepository;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (student is null || !BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return _jwtService.GenerateToken(student);
    }
}
