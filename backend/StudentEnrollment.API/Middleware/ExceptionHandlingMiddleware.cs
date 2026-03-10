using System.Net;
using System.Text.Json;
using FluentValidation;
using StudentEnrollment.Domain.Exceptions;

namespace StudentEnrollment.API.Middleware;

public class MiddlewareExcepciones
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<MiddlewareExcepciones> _logger;

    public MiddlewareExcepciones(RequestDelegate siguiente, ILogger<MiddlewareExcepciones> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _siguiente(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var errores = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { errores }));
        }
        catch (ExcepcionDominio ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Ocurrió un error inesperado." }));
        }
    }
}
