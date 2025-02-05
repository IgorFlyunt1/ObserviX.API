using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluentValidation;
using ObserviX.Shared.Entities;


namespace ObserviX.Shared.Middlewares;

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
            await HandleException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred after response started");
            throw;
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        (int statusCode, string message) = ex switch
        {
            ValidationException validationEx =>
                (StatusCodes.Status400BadRequest,
                    $"Validation failed: {string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage))}"),

            BadHttpRequestException badRequestEx =>
                (StatusCodes.Status400BadRequest, badRequestEx.Message),

            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized access"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found"),

            _ => HandleUnrecognizedError(ex)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.ErrorResponse(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment() || _env.IsEnvironment("Local")
        });

        await context.Response.WriteAsync(json);
    }

    private (int statusCode, string message) HandleUnrecognizedError(Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occurred");

        return _env.IsDevelopment() || _env.IsEnvironment("Local")
            ? (StatusCodes.Status500InternalServerError, ex.ToString())
            : (StatusCodes.Status500InternalServerError, "An unexpected error occurred");
    }
}