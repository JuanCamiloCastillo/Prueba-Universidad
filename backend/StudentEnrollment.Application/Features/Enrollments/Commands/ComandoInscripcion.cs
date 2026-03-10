using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public record ComandoInscripcion(int IdAsignatura, int IdEstudiante) : IRequest<int>;
