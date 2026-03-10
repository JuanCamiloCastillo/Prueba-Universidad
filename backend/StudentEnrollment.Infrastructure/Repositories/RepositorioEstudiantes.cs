using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class RepositorioEstudiantes : IRepositorioEstudiantes
{
    private readonly ContextoBD _contexto;

    public RepositorioEstudiantes(ContextoBD contexto)
    {
        _contexto = contexto;
    }

    public async Task<Estudiante?> ObtenerPorEmailAsync(string email, CancellationToken ct = default)
        => await _contexto.Estudiantes.FirstOrDefaultAsync(s => s.Email == email, ct);

    public async Task<Estudiante?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        => await _contexto.Estudiantes.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Estudiante>> ObtenerTodosAsync(CancellationToken ct = default)
        => await _contexto.Estudiantes.AsNoTracking().ToListAsync(ct);

    public async Task AgregarAsync(Estudiante estudiante, CancellationToken ct = default)
        => await _contexto.Estudiantes.AddAsync(estudiante, ct);

    public async Task GuardarCambiosAsync(CancellationToken ct = default)
        => await _contexto.SaveChangesAsync(ct);
}
