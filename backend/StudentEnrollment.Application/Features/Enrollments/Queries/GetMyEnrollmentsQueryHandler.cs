using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public class GetMyEnrollmentsQueryHandler : IRequestHandler<GetMyEnrollmentsQuery, IEnumerable<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetMyEnrollmentsQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IEnumerable<EnrollmentDto>> Handle(GetMyEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        return enrollments.Select(e => new EnrollmentDto(
            e.Id,
            e.SubjectId,
            e.Subject.Name,
            e.Subject.Professor.Name,
            e.EnrolledAt
        ));
    }
}
