using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public record DtoCompanero(string Nombre, string Apellido);

public record ConsultaCompaneros(int IdAsignatura, int IdEstudianteSolicitante) : IRequest<IEnumerable<DtoCompanero>>;
