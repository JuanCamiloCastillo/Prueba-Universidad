using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class ManejadorInicioSesion : IRequestHandler<ComandoInicioSesion, string>
{
    private readonly IRepositorioEstudiantes _repositorioEstudiantes;
    private readonly IServicioJwt _servicioJwt;

    public ManejadorInicioSesion(IRepositorioEstudiantes repositorioEstudiantes, IServicioJwt servicioJwt)
    {
        _repositorioEstudiantes = repositorioEstudiantes;
        _servicioJwt = servicioJwt;
    }

    public async Task<string> Handle(ComandoInicioSesion request, CancellationToken cancellationToken)
    {
        var estudiante = await _repositorioEstudiantes.ObtenerPorEmailAsync(request.Email, cancellationToken);
        if (estudiante is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, estudiante.HashContrasena))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        return _servicioJwt.GenerarToken(estudiante);
    }
}
