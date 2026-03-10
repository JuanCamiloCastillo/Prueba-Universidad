using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public class ManejadorConsultaMisInscripciones : IRequestHandler<ConsultaMisInscripciones, IEnumerable<DtoInscripcion>>
{
    private readonly IRepositorioInscripciones _repositorioInscripciones;

    public ManejadorConsultaMisInscripciones(IRepositorioInscripciones repositorioInscripciones)
    {
        _repositorioInscripciones = repositorioInscripciones;
    }

    public async Task<IEnumerable<DtoInscripcion>> Handle(ConsultaMisInscripciones request, CancellationToken cancellationToken)
    {
        var inscripciones = await _repositorioInscripciones.ObtenerPorIdEstudianteAsync(request.IdEstudiante, cancellationToken);
        return inscripciones.Select(e => new DtoInscripcion(
            e.Id,
            e.IdAsignatura,
            e.Asignatura.Nombre,
            e.Asignatura.Profesor.Nombre,
            e.FechaInscripcion
        ));
    }
}
