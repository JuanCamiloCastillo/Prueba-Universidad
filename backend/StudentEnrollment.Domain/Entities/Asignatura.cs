namespace StudentEnrollment.Domain.Entities;

public class Asignatura
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; } = 3;
    public int IdProfesor { get; set; }
    public int CapacidadMaxima { get; set; } = 30;
    public Profesor Profesor { get; set; } = null!;
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
