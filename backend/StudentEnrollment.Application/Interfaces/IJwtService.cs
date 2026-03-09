using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Student student);
}
