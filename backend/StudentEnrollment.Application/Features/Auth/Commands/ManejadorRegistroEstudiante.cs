using MediatR;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Features.Auth.Commands;

public class ManejadorRegistroEstudiante : IRequestHandler<ComandoRegistroEstudiante, int>
{
    private readonly IRepositorioEstudiantes _repositorioEstudiantes;

    public ManejadorRegistroEstudiante(IRepositorioEstudiantes repositorioEstudiantes)
    {
        _repositorioEstudiantes = repositorioEstudiantes;
    }

    public async Task<int> Handle(ComandoRegistroEstudiante request, CancellationToken cancellationToken)
    {
        var existente = await _repositorioEstudiantes.ObtenerPorEmailAsync(request.Email, cancellationToken);
        if (existente is not null)
            throw new InvalidOperationException("Este correo ya está registrado.");

        var estudiante = new Estudiante
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,
            HashContrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            FechaCreacion = DateTime.UtcNow
        };

        await _repositorioEstudiantes.AgregarAsync(estudiante, cancellationToken);
        await _repositorioEstudiantes.GuardarCambiosAsync(cancellationToken);
        return estudiante.Id;
    }
}
