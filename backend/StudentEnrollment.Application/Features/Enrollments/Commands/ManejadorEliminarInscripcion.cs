using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public class ManejadorEliminarInscripcion : IRequestHandler<ComandoEliminarInscripcion>
{
    private readonly IRepositorioInscripciones _repositorio;

    public ManejadorEliminarInscripcion(IRepositorioInscripciones repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task Handle(ComandoEliminarInscripcion request, CancellationToken cancellationToken)
        => await _repositorio.EliminarAsync(request.IdInscripcion, request.IdEstudiante, cancellationToken);
}
