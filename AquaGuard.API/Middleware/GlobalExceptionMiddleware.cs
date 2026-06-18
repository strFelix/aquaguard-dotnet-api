using AquaGuard.API.ViewModels.Shared;
using System.Net;
using System.Text.Json;

namespace AquaGuard.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Erro não tratado capturado pelo middleware global: {Message}", ex.Message);
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode = exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        ErrorResponseViewModel resposta = new ErrorResponseViewModel
        {
            Success = false,
            Message = ObterMensagem(exception, statusCode),
            StatusCode = (int)statusCode,
            Timestamp = DateTime.UtcNow
        };

        string json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static string ObterMensagem(Exception exception, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.InternalServerError)
            return "Ocorreu um erro interno no servidor. Tente novamente mais tarde.";

        return exception.Message;
    }
}