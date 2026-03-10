using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Features.Enrollments.Commands;
using StudentEnrollment.Application.Features.Enrollments.Queries;

namespace StudentEnrollment.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetStudentId() =>
        int.Parse(User.FindFirstValue("studentId") ?? throw new UnauthorizedAccessException("Student ID not found in token."));

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request, CancellationToken ct)
    {
        var studentId = GetStudentId();
        await _mediator.Send(new EnrollStudentCommand(request.SubjectId, studentId), ct);
        return Ok(new { message = "Enrollment successful." });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken ct)
    {
        var studentId = GetStudentId();
        var result = await _mediator.Send(new GetMyEnrollmentsQuery(studentId), ct);
        return Ok(result);
    }

    [HttpGet("classmates/{subjectId:int}")]
    public async Task<IActionResult> GetClassmates(int subjectId, CancellationToken ct)
    {
        var studentId = GetStudentId();
        var result = await _mediator.Send(new GetClassmatesQuery(subjectId, studentId), ct);
        return Ok(result);
    }
}

public record EnrollRequest(int SubjectId);
