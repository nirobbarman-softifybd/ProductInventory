using System.Net;
using System.Text.Json;

namespace ProductInventory.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Guid.NewGuid().ToString();
            context.Items["TraceId"] = traceId;
            try
            {
                // Add traceId to logging context
                _logger.LogInformation("TraceId: {TraceId}", traceId);
                await _next(context);

                // Handle 405 Method Not Allowed (if applicable)
                if (context.Response.StatusCode == (int)HttpStatusCode.MethodNotAllowed)
                {
                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Title = "Method Not Allowed",
                        Message = "The HTTP method used is not allowed for this resource."
                    };

                    var responseJson = JsonSerializer.Serialize(response);

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseJson);
                }
            }
            catch (Exception ex)
            {
                //await HandleExceptionAsync(context, ex);
                await HandleExceptionAsync(context, ex, traceId);
            }
        }

        //private Task HandleExceptionAsync(HttpContext context, Exception exception)
        private Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
        {
            // Log the exception (implement logging as needed)
            _logger.LogError(exception, "An unexpected error occurred. TraceId: {TraceId}", traceId);
            // e.g., _logger.LogError(exception, "An unexpected error occurred.");

            var (statusCode, title, message) = GetExceptionDetails(exception);

            var response = new
            {
                StatusCode = statusCode,
                Title = title,
                Message = message
            };

            var responseJson = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(responseJson);
        }
        private (int statusCode, string title, string message) GetExceptionDetails(Exception exception)
        {
            // Default to Internal Server Error
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Internal Server Error";
            string message = "An unexpected error occurred.";

            switch (exception)
            {
                case TaskCanceledException:
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                    title = "Request Timeout";
                    message = "The request was canceled due to a timeout.";
                    break;
                case FileNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    title = "File Not Found";
                    message = "The requested file could not be found.";
                    break;
                case FormatException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    title = "Bad Request";
                    message = "The request contains invalid data.";
                    break;
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    title = "Unauthorized";
                    message = "You are not authorized to access this resource.";
                    break;
                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    title = "Invalid Operation";
                    message = "The operation is not valid.";
                    break;
                    // Add more specific exceptions as needed
            }
            return (statusCode, title, message);
        }
    }
}
