namespace StudentEnrollment.Domain.Entities;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public Student Student { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
