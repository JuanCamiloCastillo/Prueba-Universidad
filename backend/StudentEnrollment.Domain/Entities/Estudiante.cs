namespace StudentEnrollment.Domain.Entities;

public class Estudiante
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashContrasena { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    [System.ComponentModel.DataAnnotations.Timestamp]
    public byte[] VersionFila { get; set; } = Array.Empty<byte>();
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
