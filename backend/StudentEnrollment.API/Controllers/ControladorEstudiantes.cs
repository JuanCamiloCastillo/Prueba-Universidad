using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Students.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class ControladorEstudiantes : ControllerBase
{
    private readonly IMediator _mediador;

    public ControladorEstudiantes(IMediator mediador)
    {
        _mediador = mediador;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerEstudiantes(CancellationToken ct)
    {
        var resultado = await _mediador.Send(new ConsultaEstudiantes(), ct);
        return Ok(resultado);
    }
}
