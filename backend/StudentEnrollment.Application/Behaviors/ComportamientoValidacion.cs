using FluentValidation;
using MediatR;

namespace StudentEnrollment.Application.Behaviors;

public class ComportamientoValidacion<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validadores;

    public ComportamientoValidacion(IEnumerable<IValidator<TRequest>> validadores)
    {
        _validadores = validadores;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validadores.Any())
        {
            var contexto = new ValidationContext<TRequest>(request);
            var resultados = await Task.WhenAll(
                _validadores.Select(v => v.ValidateAsync(contexto, cancellationToken)));
            var fallos = resultados
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (fallos.Count != 0)
                throw new ValidationException(fallos);
        }

        return await next();
    }
}
