using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Enrollments.Queries;
using StudentEnrollment.Application.Features.Subjects.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/asignaturas")]
[Authorize]
public class ControladorAsignaturas : ControllerBase
{
    private readonly IMediator _mediador;

    public ControladorAsignaturas(IMediator mediador)
    {
        _mediador = mediador;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerAsignaturas(CancellationToken ct)
    {
        var resultado = await _mediador.Send(new ConsultaAsignaturas(), ct);
        return Ok(resultado);
    }

    [HttpGet("{idAsignatura:int}/companeros")]
    public async Task<IActionResult> ObtenerCompaneros(int idAsignatura, CancellationToken ct)
    {
        var idEstudiante = int.Parse(User.FindFirstValue("studentId")
            ?? throw new UnauthorizedAccessException());
        var resultado = await _mediador.Send(new ConsultaCompaneros(idAsignatura, idEstudiante), ct);
        return Ok(resultado);
    }
}
