using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Subjects.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/subjects")]
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
}
