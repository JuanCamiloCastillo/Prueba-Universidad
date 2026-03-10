using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Data;

public class ContextoBD : DbContext
{
    public ContextoBD(DbContextOptions<ContextoBD> options) : base(options) { }

    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Profesor> Profesores => Set<Profesor>();
    public DbSet<Asignatura> Asignaturas => Set<Asignatura>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Estudiante>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
            e.Property(s => s.Apellido).HasMaxLength(100).IsRequired();
            e.Property(s => s.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(s => s.Email).IsUnique();
            e.Property(s => s.VersionFila).IsRowVersion();
        });

        modelBuilder.Entity<Profesor>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Asignatura>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Nombre).HasMaxLength(150).IsRequired();
            e.Property(s => s.Creditos).HasDefaultValue(3);
            e.Property(s => s.CapacidadMaxima).HasDefaultValue(30);
            e.HasOne(s => s.Profesor)
             .WithMany(p => p.Asignaturas)
             .HasForeignKey(s => s.IdProfesor);
        });

        modelBuilder.Entity<Inscripcion>(e =>
        {
            e.HasKey(en => en.Id);
            e.HasIndex(en => new { en.IdEstudiante, en.IdAsignatura }).IsUnique();
            e.HasOne(en => en.Estudiante)
             .WithMany(s => s.Inscripciones)
             .HasForeignKey(en => en.IdEstudiante);
            e.HasOne(en => en.Asignatura)
             .WithMany(s => s.Inscripciones)
             .HasForeignKey(en => en.IdAsignatura);
        });
    }
}
