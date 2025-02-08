using Microsoft.AspNetCore.Http;

namespace ObserviX.Shared.Middlewares;

public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    public TenantValidationMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) ||
            !Guid.TryParse(tenantHeader.FirstOrDefault(), out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Tenant ID missing or invalid in request.");
            return;
        }
        context.Items["TenantId"] = tenantId;
        await _next(context);
    }
}