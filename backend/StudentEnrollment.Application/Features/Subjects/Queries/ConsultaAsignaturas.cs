using MediatR;

namespace StudentEnrollment.Application.Features.Subjects.Queries;

public record DtoAsignatura(int Id, string Nombre, int Creditos, string NombreProfesor, int CapacidadMaxima, int CantidadInscritos);

public record ConsultaAsignaturas : IRequest<IEnumerable<DtoAsignatura>>;
