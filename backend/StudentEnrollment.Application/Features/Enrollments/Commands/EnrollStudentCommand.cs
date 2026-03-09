using MediatR;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public record EnrollStudentCommand(int SubjectId, int StudentId) : IRequest<int>;
