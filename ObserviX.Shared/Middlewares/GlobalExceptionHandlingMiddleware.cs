using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ObserviX.Shared.Entities;

namespace ObserviX.Shared.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                // In cases where the response has already started,
                // log the exception and rethrow.
                _logger.LogError(ex, "Exception occurred after the response started.");
                throw;
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Retrieve or generate the correlation ID.
            var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
                ? context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                : Guid.NewGuid().ToString();

            // Retrieve the tenant ID if present.
            var tenantId = context.Request.Headers.ContainsKey("X-Tenant-Id")
                ? context.Request.Headers["X-Tenant-Id"].FirstOrDefault()
                : "unknown";

            var requestContext = new
            {
                CorrelationId = correlationId,
                TenantId = tenantId,
                Path = context.Request.Path.ToString(),
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.ToString()
            };

            (int statusCode, string message) = ex switch
            {
                ValidationException validationEx => (
                    StatusCodes.Status400BadRequest,
                    $"Validation failed: {string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage))}"),
                BadHttpRequestException badRequestEx => (
                    StatusCodes.Status400BadRequest, badRequestEx.Message),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized, "Unauthorized access"),
                KeyNotFoundException => (
                    StatusCodes.Status404NotFound, "Resource not found"),
                _ => HandleUnrecognizedError(ex, requestContext)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            context.Response.Headers["X-Correlation-ID"] = correlationId;
            context.Response.Headers["X-Tenant-Id"] = tenantId;

            var response = ApiResponse<object>.ErrorResponse(message);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment() || _env.IsEnvironment("Local")
            });

            await context.Response.WriteAsync(json);
        }

        private (int statusCode, string message) HandleUnrecognizedError(Exception ex, object requestContext)
        {
            _logger.LogError(ex, "Unhandled exception occurred. Request details: {@RequestContext}", requestContext);

            // return _env.IsDevelopment() || _env.IsEnvironment("Local")
            //     ? (StatusCodes.Status500InternalServerError, ex.ToString())
            //     : (StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            
            return (StatusCodes.Status500InternalServerError, ex.ToString());
        }
    }
}