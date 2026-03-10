using MediatR;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Exceptions;

namespace StudentEnrollment.Application.Features.Enrollments.Commands;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, int>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ISubjectRepository _subjectRepository;

    public EnrollStudentCommandHandler(IEnrollmentRepository enrollmentRepository, ISubjectRepository subjectRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<int> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);
        if (subject is null)
            throw new DomainException("Subject not found.");

        await _enrollmentRepository.EnrollStudentAsync(request.StudentId, request.SubjectId, cancellationToken);
        return request.SubjectId;
    }
}
