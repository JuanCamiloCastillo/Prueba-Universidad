using MediatR;

namespace StudentEnrollment.Application.Features.Students.Queries;

public record StudentPublicDto(int Id, string FirstName, string LastName);

public record GetStudentsQuery : IRequest<IEnumerable<StudentPublicDto>>;
