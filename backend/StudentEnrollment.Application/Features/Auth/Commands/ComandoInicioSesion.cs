using MediatR;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public record ComandoInicioSesion(string Email, string Contrasena) : IRequest<string>;
