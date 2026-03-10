namespace StudentEnrollment.Domain.Entities;

public class Inscripcion
{
    public int Id { get; set; }
    public int IdEstudiante { get; set; }
    public int IdAsignatura { get; set; }
    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    public Estudiante Estudiante { get; set; } = null!;
    public Asignatura Asignatura { get; set; } = null!;
}
