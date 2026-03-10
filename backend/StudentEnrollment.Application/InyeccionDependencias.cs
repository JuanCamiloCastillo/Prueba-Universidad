using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StudentEnrollment.Application.Behaviors;

namespace StudentEnrollment.Application;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InyeccionDependencias).Assembly));
        services.AddValidatorsFromAssembly(typeof(InyeccionDependencias).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ComportamientoValidacion<,>));
        return services;
    }
}
