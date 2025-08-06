using Penni.Application.Common.Exceptions;
using Penni.WebAPI.Models.Responses;
using System.Text.Json;

namespace Penni.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (PenniException ex)
            {
                _logger.LogError(ex, "❌ HTTP exception occurred");

                var errorResponse = new ErrorResponseDto
                {
                    StatusCode = ex.StatusCode,
                    MessageCode = ex.MessageCode,
                    Message = ex.Message
                };

                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unhandled exception occurred");

                var errorResponse = new ErrorResponseDto
                {
                    StatusCode = 500,
                    MessageCode = "INTERNAL_SERVER_ERROR",
                    Message = "An unexpected error occurred."
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
