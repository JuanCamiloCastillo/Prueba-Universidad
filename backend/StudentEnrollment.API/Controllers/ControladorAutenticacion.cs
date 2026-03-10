using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Auth.Commands;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/auth")]
public class ControladorAutenticacion : ControllerBase
{
    private readonly IMediator _mediador;

    public ControladorAutenticacion(IMediator mediador)
    {
        _mediador = mediador;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registrar([FromBody] ComandoRegistroEstudiante comando, CancellationToken ct)
    {
        var id = await _mediador.Send(comando, ct);
        return CreatedAtAction(nameof(Registrar), new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion([FromBody] ComandoInicioSesion comando, CancellationToken ct)
    {
        var token = await _mediador.Send(comando, ct);
        return Ok(new { token });
    }
}
