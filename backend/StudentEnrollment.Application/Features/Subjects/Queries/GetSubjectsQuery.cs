using MediatR;

namespace StudentEnrollment.Application.Features.Subjects.Queries;

public record SubjectDto(int Id, string Name, int Credits, string ProfessorName, int MaxCapacity, int EnrolledCount);

public record GetSubjectsQuery : IRequest<IEnumerable<SubjectDto>>;
