using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IServicioJwt
{
    string GenerarToken(Estudiante estudiante);
}
