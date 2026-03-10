using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public record DtoInscripcion(int Id, int IdAsignatura, string NombreAsignatura, string NombreProfesor, DateTime FechaInscripcion);

public record ConsultaMisInscripciones(int IdEstudiante) : IRequest<IEnumerable<DtoInscripcion>>;
