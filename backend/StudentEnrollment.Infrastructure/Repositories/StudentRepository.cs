using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Students.FirstOrDefaultAsync(s => s.Email == email, ct);

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Students.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default)
        => await _context.Students.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Student student, CancellationToken ct = default)
        => await _context.Students.AddAsync(student, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
