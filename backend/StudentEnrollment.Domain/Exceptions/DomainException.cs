namespace StudentEnrollment.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class EnrollmentLimitExceededException : DomainException
{
    public EnrollmentLimitExceededException() : base("Student cannot enroll in more than 3 subjects.") { }
}

public class DuplicateProfessorException : DomainException
{
    public DuplicateProfessorException() : base("Student already has a class with this professor.") { }
}

public class SubjectFullException : DomainException
{
    public SubjectFullException() : base("Subject has reached its maximum capacity.") { }
}

public class AlreadyEnrolledException : DomainException
{
    public AlreadyEnrolledException() : base("Student is already enrolled in this subject.") { }
}
