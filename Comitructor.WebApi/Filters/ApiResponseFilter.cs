using Comitructor.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Comitructor.WebApi.Filters
{
    public class ApiResponseFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value != null &&
                    objectResult.Value.GetType().IsGenericType &&
                    objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    await next();
                    return;
                }

                var response = new ApiResponse<object>
                {
                    Success = context.HttpContext.Response.StatusCode >= 200 &&
                              context.HttpContext.Response.StatusCode < 300,
                    Message = "Operación realizada con éxito",
                    Data = objectResult.Value,
                    Timestamp = DateTime.UtcNow
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
            }

            await next();
        }
    }
}