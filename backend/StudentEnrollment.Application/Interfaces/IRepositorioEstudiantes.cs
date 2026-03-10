using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IRepositorioEstudiantes
{
    Task<Estudiante?> ObtenerPorEmailAsync(string email, CancellationToken ct = default);
    Task<Estudiante?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Estudiante>> ObtenerTodosAsync(CancellationToken ct = default);
    Task AgregarAsync(Estudiante estudiante, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}
