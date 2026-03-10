using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IRepositorioAsignaturas
{
    Task<IEnumerable<Asignatura>> ObtenerTodasConDetallesAsync(CancellationToken ct = default);
    Task<Asignatura?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
}
