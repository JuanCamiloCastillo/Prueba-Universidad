using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Subjects.Queries;

public class ManejadorConsultaAsignaturas : IRequestHandler<ConsultaAsignaturas, IEnumerable<DtoAsignatura>>
{
    private readonly IRepositorioAsignaturas _repositorioAsignaturas;

    public ManejadorConsultaAsignaturas(IRepositorioAsignaturas repositorioAsignaturas)
    {
        _repositorioAsignaturas = repositorioAsignaturas;
    }

    public async Task<IEnumerable<DtoAsignatura>> Handle(ConsultaAsignaturas request, CancellationToken cancellationToken)
    {
        var asignaturas = await _repositorioAsignaturas.ObtenerTodasConDetallesAsync(cancellationToken);
        return asignaturas.Select(s => new DtoAsignatura(
            s.Id,
            s.Nombre,
            s.Creditos,
            s.Profesor.Nombre,
            s.CapacidadMaxima,
            s.Inscripciones.Count
        ));
    }
}
