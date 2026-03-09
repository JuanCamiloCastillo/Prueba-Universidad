using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<Subject?> GetByIdAsync(int id, CancellationToken ct = default);
}
