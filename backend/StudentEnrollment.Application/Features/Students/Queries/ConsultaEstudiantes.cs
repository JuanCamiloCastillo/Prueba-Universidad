using MediatR;

namespace StudentEnrollment.Application.Features.Students.Queries;

public record DtoEstudiantePublico(int Id, string Nombre, string Apellido);

public record ConsultaEstudiantes : IRequest<IEnumerable<DtoEstudiantePublico>>;
