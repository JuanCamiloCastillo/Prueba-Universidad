using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Students.Queries;

public class ManejadorConsultaEstudiantes : IRequestHandler<ConsultaEstudiantes, IEnumerable<DtoEstudiantePublico>>
{
    private readonly IRepositorioEstudiantes _repositorioEstudiantes;

    public ManejadorConsultaEstudiantes(IRepositorioEstudiantes repositorioEstudiantes)
    {
        _repositorioEstudiantes = repositorioEstudiantes;
    }

    public async Task<IEnumerable<DtoEstudiantePublico>> Handle(ConsultaEstudiantes request, CancellationToken cancellationToken)
    {
        var estudiantes = await _repositorioEstudiantes.ObtenerTodosAsync(cancellationToken);
        return estudiantes.Select(s => new DtoEstudiantePublico(s.Id, s.Nombre, s.Apellido));
    }
}
