using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentEnrollment.Application.Interfaces;

namespace StudentEnrollment.Infrastructure.Services;

public class ServicioCache : IServicioCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ServicioCache> _logger;
    private readonly TimeSpan _expiracionPorDefecto;

    public ServicioCache(IDistributedCache cache, ILogger<ServicioCache> logger, IConfiguration configuracion)
    {
        _cache = cache;
        _logger = logger;
        var minutos = configuracion.GetValue<int?>("CacheSettings:DefaultExpirationMinutes") ?? 5;
        _expiracionPorDefecto = TimeSpan.FromMinutes(minutos);
    }

    public async Task<T?> ObtenerAsync<T>(string clave, CancellationToken ct = default)
    {
        var valor = await _cache.GetStringAsync(clave, ct);
        if (valor is null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(valor);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "No se pudo deserializar el valor en caché para la clave '{Clave}'. Removiendo entrada corrupta.", clave);
            await _cache.RemoveAsync(clave, ct);
            return default;
        }
    }

    public async Task EstablecerAsync<T>(string clave, T valor, TimeSpan? expiracion = null, CancellationToken ct = default)
    {
        var opciones = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiracion ?? _expiracionPorDefecto
        };
        await _cache.SetStringAsync(clave, JsonSerializer.Serialize(valor), opciones, ct);
    }

    public async Task RemoverAsync(string clave, CancellationToken ct = default)
        => await _cache.RemoveAsync(clave, ct);
}
