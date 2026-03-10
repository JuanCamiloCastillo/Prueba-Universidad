using MediatR;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public record ComandoRegistroEstudiante(string Nombre, string Apellido, string Email, string Contrasena) : IRequest<int>;
