using Comitructor.Domain.Exceptions;
using Comitructor.Infrastructure.Common;
using System.Net;
using System.Text.Json;

namespace Comitructor.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                UserFriendlyException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = exception.Message,
                Data = _env.IsDevelopment() ? exception.StackTrace : null,
                Timestamp = DateTime.UtcNow
            };

            if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError && !_env.IsDevelopment())
            {
                response.Message = "Ha ocurrido un error interno en el servidor. Por favor, intente más tarde.";
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Serialize(response, jsonOptions);

            await context.Response.WriteAsync(result);
        }
    }
}