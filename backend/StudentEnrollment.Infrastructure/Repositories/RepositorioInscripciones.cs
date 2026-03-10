using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class RepositorioInscripciones : IRepositorioInscripciones
{
    private readonly ContextoBD _contexto;

    public RepositorioInscripciones(ContextoBD contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerPorIdEstudianteAsync(int idEstudiante, CancellationToken ct = default)
        => await _contexto.Inscripciones
            .AsNoTracking()
            .Include(e => e.Asignatura)
            .ThenInclude(s => s.Profesor)
            .Where(e => e.IdEstudiante == idEstudiante)
            .ToListAsync(ct);

    public async Task<IEnumerable<Inscripcion>> ObtenerPorIdAsignaturaAsync(int idAsignatura, CancellationToken ct = default)
        => await _contexto.Inscripciones
            .AsNoTracking()
            .Include(e => e.Estudiante)
            .Where(e => e.IdAsignatura == idAsignatura)
            .ToListAsync(ct);

    public async Task InscribirEstudianteAsync(int idEstudiante, int idAsignatura, CancellationToken ct = default)
    {
        try
        {
            await _contexto.Database.ExecuteSqlRawAsync(
                "EXEC sp_EnrollStudent @StudentId, @SubjectId",
                new SqlParameter("@StudentId", idEstudiante),
                new SqlParameter("@SubjectId", idAsignatura));
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new ExcepcionLimiteInscripciones();
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new ExcepcionProfesorDuplicado();
        }
        catch (SqlException ex) when (ex.Number == 50003)
        {
            throw new ExcepcionAsignaturaLlena();
        }
        catch (SqlException ex) when (ex.Number == 50004)
        {
            throw new ExcepcionYaInscrito();
        }
    }
}
