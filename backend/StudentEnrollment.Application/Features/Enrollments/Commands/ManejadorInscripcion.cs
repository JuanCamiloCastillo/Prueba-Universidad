using MediatR;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Exceptions;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public class ManejadorInscripcion : IRequestHandler<ComandoInscripcion, int>
{
    private readonly IRepositorioInscripciones _repositorioInscripciones;
    private readonly IRepositorioAsignaturas _repositorioAsignaturas;

    public ManejadorInscripcion(IRepositorioInscripciones repositorioInscripciones, IRepositorioAsignaturas repositorioAsignaturas)
    {
        _repositorioInscripciones = repositorioInscripciones;
        _repositorioAsignaturas = repositorioAsignaturas;
    }

    public async Task<int> Handle(ComandoInscripcion request, CancellationToken cancellationToken)
    {
        var asignatura = await _repositorioAsignaturas.ObtenerPorIdAsync(request.IdAsignatura, cancellationToken);
        if (asignatura is null)
            throw new ExcepcionDominio("No se encontró la materia.");

        await _repositorioInscripciones.InscribirEstudianteAsync(request.IdEstudiante, request.IdAsignatura, cancellationToken);
        return request.IdAsignatura;
    }
}
