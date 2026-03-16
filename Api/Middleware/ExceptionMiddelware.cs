using Domain.Common;
using Domain.Exceptions;
using System.Text.Json;

namespace Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                TaskCanceledException => StatusCodes.Status408RequestTimeout,
                BaseException baseEx => baseEx.StatusCode,
                _ => StatusCodes.Status500InternalServerError
            };

            var apiResponse = exception switch
            {
                BaseException baseEx => ApiResponse<object>.ErrorResult(
                    message: baseEx.Message,
                    errors: baseEx is ValidationException valEx ? valEx.ValidationErrors : Array.Empty<string>(),
                    statusCode: statusCode,
                    errorCode: baseEx.ErrorCode,
                    requestId: context.TraceIdentifier
                ),
                _ => ApiResponse<object>.ErrorResult(
                    message: "An internal server error occurred",
                    errors: Array.Empty<string>(),
                    statusCode: statusCode,
                    errorCode: "UNHANDLED_EXCEPTION",
                    requestId: context.TraceIdentifier
                )
            };

            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
