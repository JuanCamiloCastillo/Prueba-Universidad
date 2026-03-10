using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Enrollments.Queries;

public class GetClassmatesQueryHandler : IRequestHandler<GetClassmatesQuery, IEnumerable<ClassmateDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetClassmatesQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IEnumerable<ClassmateDto>> Handle(GetClassmatesQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetBySubjectIdAsync(request.SubjectId, cancellationToken);
        return enrollments
            .Where(e => e.StudentId != request.RequestingStudentId)
            .Select(e => new ClassmateDto(e.Student.FirstName, e.Student.LastName));
    }
}
