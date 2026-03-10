namespace StudentEnrollment.Application.Interfaces;

public interface IServicioCache
{
    Task<T?> ObtenerAsync<T>(string clave, CancellationToken ct = default);
    Task EstablecerAsync<T>(string clave, T valor, TimeSpan? expiracion = null, CancellationToken ct = default);
    Task RemoverAsync(string clave, CancellationToken ct = default);
}
