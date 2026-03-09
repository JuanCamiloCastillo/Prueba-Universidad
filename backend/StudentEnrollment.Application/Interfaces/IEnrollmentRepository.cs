using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<IEnumerable<Enrollment>> GetBySubjectIdAsync(int subjectId, CancellationToken ct = default);
    Task EnrollStudentAsync(int studentId, int subjectId, CancellationToken ct = default);
}
