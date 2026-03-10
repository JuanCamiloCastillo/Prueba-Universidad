namespace StudentEnrollment.Domain.Entities;

public class Profesor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Asignatura> Asignaturas { get; set; } = new List<Asignatura>();
}
