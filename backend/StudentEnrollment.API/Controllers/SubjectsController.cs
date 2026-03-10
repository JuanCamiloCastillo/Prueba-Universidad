using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Subjects.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/subjects")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSubjectsQuery(), ct);
        return Ok(result);
    }
}
