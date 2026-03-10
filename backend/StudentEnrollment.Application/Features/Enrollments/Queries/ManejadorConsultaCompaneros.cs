using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public class ManejadorConsultaCompaneros : IRequestHandler<ConsultaCompaneros, IEnumerable<DtoCompanero>>
{
    private readonly IRepositorioInscripciones _repositorioInscripciones;

    public ManejadorConsultaCompaneros(IRepositorioInscripciones repositorioInscripciones)
    {
        _repositorioInscripciones = repositorioInscripciones;
    }

    public async Task<IEnumerable<DtoCompanero>> Handle(ConsultaCompaneros request, CancellationToken cancellationToken)
    {
        var inscripciones = await _repositorioInscripciones.ObtenerPorIdAsignaturaAsync(request.IdAsignatura, cancellationToken);
        return inscripciones
            .Where(e => e.IdEstudiante != request.IdEstudianteSolicitante)
            .Select(e => new DtoCompanero(e.Estudiante.Nombre, e.Estudiante.Apellido));
    }
}
