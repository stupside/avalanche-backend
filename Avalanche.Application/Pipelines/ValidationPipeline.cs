using FluentValidation;
using FluentValidation.Results;
using MediatR.Pipeline;

namespace Avalanche.Application.Pipelines;

public class ValidationPipeline<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<ValidationFailure> failures = new List<ValidationFailure>();

        var failed = false;

        foreach (var validator in _validators.ToArray())
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (result.IsValid) continue;

            failures = failures.Concat(result.Errors);

            failed = true;
        }

        if (failed)
            throw new ValidationException(failures);
    }
}