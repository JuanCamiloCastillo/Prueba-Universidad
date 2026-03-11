using StudentEnrollment.Application.Features.Enrollments.Queries;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IRepositorioInscripciones
{
    Task<IEnumerable<Inscripcion>> ObtenerPorIdEstudianteAsync(int idEstudiante, CancellationToken ct = default);
    Task<IEnumerable<Inscripcion>> ObtenerPorIdAsignaturaAsync(int idAsignatura, CancellationToken ct = default);
    Task InscribirEstudianteAsync(int idEstudiante, int idAsignatura, CancellationToken ct = default);
    Task<IEnumerable<DtoCompanero>> ObtenerCompanerosAsync(int idAsignatura, int idEstudianteSolicitante, CancellationToken ct = default);
}
