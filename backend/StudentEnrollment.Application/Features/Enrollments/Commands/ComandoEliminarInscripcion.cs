using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public record ComandoEliminarInscripcion(int IdInscripcion, int IdEstudiante) : IRequest;
