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

    public Task<IEnumerable<DtoCompanero>> Handle(ConsultaCompaneros request, CancellationToken cancellationToken)
        => _repositorioInscripciones.ObtenerCompanerosAsync(
               request.IdAsignatura,
               request.IdEstudianteSolicitante,
               cancellationToken);
}
