using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public record ClassmateDto(string FirstName, string LastName);

public record GetClassmatesQuery(int SubjectId, int RequestingStudentId) : IRequest<IEnumerable<ClassmateDto>>;
