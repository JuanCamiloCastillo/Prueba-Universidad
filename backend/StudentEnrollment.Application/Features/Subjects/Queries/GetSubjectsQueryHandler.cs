using MediatR;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Application.Features.Subjects.Queries;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, IEnumerable<SubjectDto>>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectsQueryHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<IEnumerable<SubjectDto>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await _subjectRepository.GetAllWithDetailsAsync(cancellationToken);
        return subjects.Select(s => new SubjectDto(
            s.Id,
            s.Name,
            s.Credits,
            s.Professor.Name,
            s.MaxCapacity,
            s.Enrollments.Count
        ));
    }
}
