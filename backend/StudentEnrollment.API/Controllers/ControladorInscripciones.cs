using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Enrollments.Commands;
using StudentEnrollment.Application.Features.Enrollments.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/inscripciones")]
[Authorize]
public class ControladorInscripciones : ControllerBase
{
    private readonly IMediator _mediador;

    public ControladorInscripciones(IMediator mediador)
    {
        _mediador = mediador;
    }

    private int ObtenerIdEstudiante() =>
        int.Parse(User.FindFirstValue("studentId") ?? throw new UnauthorizedAccessException("No se encontró el ID del estudiante en el token."));

    [HttpPost]
    public async Task<IActionResult> Inscribir([FromBody] SolicitudInscripcion solicitud, CancellationToken ct)
    {
        var idEstudiante = ObtenerIdEstudiante();
        await _mediador.Send(new ComandoInscripcion(solicitud.IdAsignatura, idEstudiante), ct);
        return Ok(new { mensaje = "Inscripción exitosa." });
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerMisInscripciones(CancellationToken ct)
    {
        var idEstudiante = ObtenerIdEstudiante();
        var resultado = await _mediador.Send(new ConsultaMisInscripciones(idEstudiante), ct);
        return Ok(resultado);
    }

    [HttpGet("companeros/{idAsignatura:int}")]
    public async Task<IActionResult> ObtenerCompaneros(int idAsignatura, CancellationToken ct)
    {
        var idEstudiante = ObtenerIdEstudiante();
        var resultado = await _mediador.Send(new ConsultaCompaneros(idAsignatura, idEstudiante), ct);
        return Ok(resultado);
    }
}

public record SolicitudInscripcion(int IdAsignatura);
