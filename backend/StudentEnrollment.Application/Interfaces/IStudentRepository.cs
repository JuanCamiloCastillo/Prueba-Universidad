using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Student?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Student student, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
