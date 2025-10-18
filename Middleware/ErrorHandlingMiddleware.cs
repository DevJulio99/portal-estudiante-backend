using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyPortalStudent.Domain;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred.");

        var statusCode = HttpStatusCode.InternalServerError; // 500 por defecto
        var message = "Ocurrió un error interno en el servidor.";

        // Personalizamos el código de estado según el tipo de excepción
        switch (exception)
        {
            case ArgumentException _:
            case InvalidOperationException _:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case UnauthorizedAccessException _:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
                // Puedes añadir más casos para excepciones personalizadas
                // case NotFoundException _:
                //     statusCode = HttpStatusCode.NotFound; // 404
                //     message = exception.Message;
                //     break;
        }

        var errorDetails = new ErrorDetails
        {
            Code = "INTERNAL_SERVER_ERROR",
            Title = "Error Inesperado",
            Description = "Ocurrió un error interno en el servidor.",
            Icon = ""
        };

        var response = new ApiResponse<object>
        {
            Flag = false,
            Success = false,
            Message = message,
            Data = null,
            Error = errorDetails
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
