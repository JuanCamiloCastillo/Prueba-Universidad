using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Subject)
            .ThenInclude(s => s.Professor)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Enrollment>> GetBySubjectIdAsync(int subjectId, CancellationToken ct = default)
        => await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Where(e => e.SubjectId == subjectId)
            .ToListAsync(ct);

    public async Task EnrollStudentAsync(int studentId, int subjectId, CancellationToken ct = default)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_EnrollStudent @StudentId, @SubjectId",
                new SqlParameter("@StudentId", studentId),
                new SqlParameter("@SubjectId", subjectId));
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new EnrollmentLimitExceededException();
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new DuplicateProfessorException();
        }
        catch (SqlException ex) when (ex.Number == 50003)
        {
            throw new SubjectFullException();
        }
        catch (SqlException ex) when (ex.Number == 50004)
        {
            throw new AlreadyEnrolledException();
        }
    }
}
