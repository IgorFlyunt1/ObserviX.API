using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ObserviX.Shared.Entities;

namespace ObserviX.Shared.Attributes;

public class ApiResponseWrapperAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        switch (context.Result)
        {
            // If the response is already in ApiResponse envelope, do nothing.
            case ObjectResult objectResult when objectResult.Value is ApiResponse<object>:
                base.OnResultExecuting(context);
                return;
            // Otherwise, wrap the result.
            case ObjectResult objectResult:
                objectResult.Value = ApiResponse<object>.SuccessResponse(objectResult.Value);
                break;
            // Handle the case of an EmptyResult.
            case EmptyResult:
                context.Result = new ObjectResult(ApiResponse<object>.SuccessResponse(null))
                {
                    StatusCode = 200
                };
                break;
        }

        base.OnResultExecuting(context);
    }
}