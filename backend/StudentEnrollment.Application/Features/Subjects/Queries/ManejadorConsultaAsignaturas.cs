using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Subjects.Queries;

public class ManejadorConsultaAsignaturas : IRequestHandler<ConsultaAsignaturas, IEnumerable<DtoAsignatura>>
{
    private readonly IRepositorioAsignaturas _repositorioAsignaturas;
    private readonly IServicioCache _cache;
    private const string ClaveCache = "asignaturas:todas";

    public ManejadorConsultaAsignaturas(IRepositorioAsignaturas repositorioAsignaturas, IServicioCache cache)
    {
        _repositorioAsignaturas = repositorioAsignaturas;
        _cache = cache;
    }

    public async Task<IEnumerable<DtoAsignatura>> Handle(ConsultaAsignaturas request, CancellationToken cancellationToken)
    {
        try
        {
            var enCache = await _cache.ObtenerAsync<List<DtoAsignatura>>(ClaveCache, cancellationToken);
            if (enCache is not null)
                return enCache;
        }
        catch { /* Redis no disponible, continua con BD */ }

        var asignaturas = await _repositorioAsignaturas.ObtenerTodasConDetallesAsync(cancellationToken);
        var resultado = asignaturas.Select(s => new DtoAsignatura(
            s.Id,
            s.Nombre,
            s.Creditos,
            s.Profesor.Nombre,
            s.CapacidadMaxima,
            s.Inscripciones.Count
        )).ToList();

        try { await _cache.EstablecerAsync(ClaveCache, resultado, TimeSpan.FromMinutes(2), cancellationToken); }
        catch { /* Redis no disponible, se omite el guardado en cache */ }

        return resultado;
    }
}
