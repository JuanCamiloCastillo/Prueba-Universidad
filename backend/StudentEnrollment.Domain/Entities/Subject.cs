namespace StudentEnrollment.Domain.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; } = 3;
    public int ProfessorId { get; set; }
    public int MaxCapacity { get; set; } = 30;
    public Professor Professor { get; set; } = null!;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
