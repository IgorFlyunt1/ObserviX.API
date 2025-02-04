using System.Diagnostics;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ObserviX.Shared.Extensions.Mediatr
{
    /// <summary>
    /// Catches any unhandled exception during the request pipeline, logs it, and then rethrows the exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing {RequestName}. Request: {@Request}",
                    typeof(TRequest).Name, request);
                throw; 
            }
        }
    }

    /// <summary>
    /// Logs the beginning and end of each request handling. 
    /// This behavior logs the request payload and the type of the request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {RequestName} with payload: {@Request}", typeof(TRequest).Name, request);

            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("Handled successfully {RequestName} in {ElapsedMilliseconds} milliseconds",
                typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

            return response;
        }
    }

    /// <summary>
    /// Measures the execution time of a request. If the request takes longer than a specified threshold,
    /// a warning is logged.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private const int WarningThresholdMilliseconds = 500; // Adjust as needed

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > WarningThresholdMilliseconds)
            {
                _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    typeof(TRequest).Name, stopwatch.ElapsedMilliseconds, request);
            }

            return response;
        }
    }

    /// <summary>
    /// Executes all FluentValidation validators for the given request. 
    /// If validation fails, a ValidationException is thrown containing all errors.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );
                var failures = validationResults.SelectMany(result => result.Errors).Where(f => f != null).ToList();

                if (failures.Any())
                {
                    // You can customize the exception type or message as needed.
                    throw new ValidationException("Validation errors occurred", failures);
                }
            }

            return await next();
        }
    }
}
