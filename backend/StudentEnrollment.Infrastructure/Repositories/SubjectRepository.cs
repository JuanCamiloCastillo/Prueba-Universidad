using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _context;

    public SubjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subject>> GetAllWithDetailsAsync(CancellationToken ct = default)
        => await _context.Subjects
            .AsNoTracking()
            .Include(s => s.Professor)
            .Include(s => s.Enrollments)
            .ToListAsync(ct);

    public async Task<Subject?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Subjects
            .Include(s => s.Professor)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
}
