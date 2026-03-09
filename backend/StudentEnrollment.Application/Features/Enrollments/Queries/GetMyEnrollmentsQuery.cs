using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public record EnrollmentDto(int Id, int SubjectId, string SubjectName, string ProfessorName, DateTime EnrolledAt);

public record GetMyEnrollmentsQuery(int StudentId) : IRequest<IEnumerable<EnrollmentDto>>;
