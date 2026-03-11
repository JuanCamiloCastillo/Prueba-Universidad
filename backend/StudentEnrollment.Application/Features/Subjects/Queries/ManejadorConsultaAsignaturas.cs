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
        var enCache = await _cache.ObtenerAsync<List<DtoAsignatura>>(ClaveCache, cancellationToken);
        if (enCache is not null)
            return enCache;

        var asignaturas = await _repositorioAsignaturas.ObtenerTodasConDetallesAsync(cancellationToken);
        var resultado = asignaturas.Select(s => new DtoAsignatura(
            s.Id,
            s.Nombre,
            s.Creditos,
            s.Profesor.Nombre,
            s.CapacidadMaxima,
            s.Inscripciones.Count
        )).ToList();

        await _cache.EstablecerAsync(ClaveCache, resultado, TimeSpan.FromMinutes(2), cancellationToken);
        return resultado;
    }
}
