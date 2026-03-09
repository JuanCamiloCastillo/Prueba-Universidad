using MediatR;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<string>;
