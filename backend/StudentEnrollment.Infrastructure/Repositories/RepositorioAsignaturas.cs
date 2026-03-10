using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class RepositorioAsignaturas : IRepositorioAsignaturas
{
    private readonly ContextoBD _contexto;

    public RepositorioAsignaturas(ContextoBD contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Asignatura>> ObtenerTodasConDetallesAsync(CancellationToken ct = default)
        => await _contexto.Asignaturas
            .AsNoTracking()
            .Include(s => s.Profesor)
            .Include(s => s.Inscripciones)
            .ToListAsync(ct);

    public async Task<Asignatura?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        => await _contexto.Asignaturas
            .Include(s => s.Profesor)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
}
