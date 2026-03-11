using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Students.Queries;

public class ManejadorConsultaEstudiantes : IRequestHandler<ConsultaEstudiantes, IEnumerable<DtoEstudiantePublico>>
{
    private readonly IRepositorioEstudiantes _repositorioEstudiantes;
    private readonly IServicioCache _cache;
    private const string ClaveCache = "estudiantes:todos";

    public ManejadorConsultaEstudiantes(IRepositorioEstudiantes repositorioEstudiantes, IServicioCache cache)
    {
        _repositorioEstudiantes = repositorioEstudiantes;
        _cache = cache;
    }

    public async Task<IEnumerable<DtoEstudiantePublico>> Handle(ConsultaEstudiantes request, CancellationToken cancellationToken)
    {
        try
        {
            var enCache = await _cache.ObtenerAsync<List<DtoEstudiantePublico>>(ClaveCache, cancellationToken);
            if (enCache is not null)
                return enCache;
        }
        catch { /* Redis no disponible, continua con BD */ }

        var estudiantes = await _repositorioEstudiantes.ObtenerTodosAsync(cancellationToken);
        var resultado = estudiantes.Select(s => new DtoEstudiantePublico(s.Id, s.Nombre, s.Apellido)).ToList();

        try { await _cache.EstablecerAsync(ClaveCache, resultado, TimeSpan.FromMinutes(5), cancellationToken); }
        catch { /* Redis no disponible, se omite el guardado en cache */ }

        return resultado;
    }
}
