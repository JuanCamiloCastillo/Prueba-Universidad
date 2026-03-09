using MediatR;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public record RegisterStudentCommand(string FirstName, string LastName, string Email, string Password) : IRequest<int>;
