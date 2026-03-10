using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.FirstName).HasMaxLength(100).IsRequired();
            e.Property(s => s.LastName).HasMaxLength(100).IsRequired();
            e.Property(s => s.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(s => s.Email).IsUnique();
            e.Property(s => s.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<Professor>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(150).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Subject>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).HasMaxLength(150).IsRequired();
            e.Property(s => s.Credits).HasDefaultValue(3);
            e.Property(s => s.MaxCapacity).HasDefaultValue(30);
            e.HasOne(s => s.Professor)
             .WithMany(p => p.Subjects)
             .HasForeignKey(s => s.ProfessorId);
        });

        modelBuilder.Entity<Enrollment>(e =>
        {
            e.HasKey(en => en.Id);
            e.HasIndex(en => new { en.StudentId, en.SubjectId }).IsUnique();
            e.HasOne(en => en.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(en => en.StudentId);
            e.HasOne(en => en.Subject)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(en => en.SubjectId);
        });
    }
}
