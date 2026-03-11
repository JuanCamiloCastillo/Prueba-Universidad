using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Services;

public class ServicioJwt : IServicioJwt
{
    private readonly IConfiguration _configuracion;

    public ServicioJwt(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public string GenerarToken(Estudiante estudiante)
    {
        var configuracionJwt = _configuracion.GetSection("JwtSettings");
        var claveSecreta = configuracionJwt["SecretKey"] ?? throw new InvalidOperationException("No se configuró la clave secreta JWT.");
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, estudiante.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, estudiante.Email),
            new Claim(JwtRegisteredClaimNames.Name, estudiante.Nombre),
            new Claim("studentId", estudiante.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var horasExpiracion = int.TryParse(configuracionJwt["TokenExpirationHours"], out var h) ? h : 8;
        var token = new JwtSecurityToken(
            issuer: configuracionJwt["Issuer"],
            audience: configuracionJwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(horasExpiracion),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
