using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentEnrollment.Application.Interfaces;
using StudentEnrollment.Infrastructure.Data;
using StudentEnrollment.Infrastructure.Repositories;
using StudentEnrollment.Infrastructure.Services;

namespace StudentEnrollment.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ContextoBD>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddScoped<IRepositorioEstudiantes, RepositorioEstudiantes>();
        services.AddScoped<IRepositorioAsignaturas, RepositorioAsignaturas>();
        services.AddScoped<IRepositorioInscripciones, RepositorioInscripciones>();
        services.AddScoped<IServicioJwt, ServicioJwt>();
        services.AddScoped<IServicioCache, ServicioCache>();

        return services;
    }
}
