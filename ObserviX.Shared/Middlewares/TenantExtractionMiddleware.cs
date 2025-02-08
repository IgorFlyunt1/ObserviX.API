using Microsoft.AspNetCore.Http;

namespace ObserviX.Shared.Middlewares;

public class TenantExtractionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? tenantIdStr = context.User?.FindFirst("tenantId")?.Value;
        if (string.IsNullOrEmpty(tenantIdStr))
            tenantIdStr = context.Request.Headers["X-Tenant-Id"].ToString();

        if (!string.IsNullOrEmpty(tenantIdStr) && Guid.TryParse(tenantIdStr, out var tenantId))
        {
            context.Request.Headers["X-Tenant-Id"] = tenantId.ToString();
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Tenant ID missing or invalid");
            return;
        }

        await next(context);
    }
}