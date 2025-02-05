using Microsoft.AspNetCore.Http;
using ObserviX.Shared.Entities;

namespace ObserviX.Shared.Extensions.ApiResponseWrapper;

public class ApiResponseWrapperFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next(context);


        if (result is IResult)
        {
            return result;
        }

        return Results.Ok(ApiResponse<object>.SuccessResponse(result));
    }
}