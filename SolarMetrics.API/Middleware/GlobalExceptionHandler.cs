using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SolarMetrics.Exceptions;

namespace SolarMetrics.Middleware;

public sealed class GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, message) = MapException(exception);

        if (status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
        else
            logger.LogWarning(exception, "Erro de negócio: {Message}", message);

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/json";

        var body = new
        {
            message,
            status,
            detail = status >= StatusCodes.Status500InternalServerError && environment.IsDevelopment()
                ? exception.ToString()
                : null
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions), cancellationToken);
        return true;
    }

    private static (int Status, string Message) MapException(Exception exception) =>
        exception switch
        {
            ClienteNaoEncontradoException or
                SistemaNaoEncontradoException or
                PainelSolarNaoEncontradoException or
                SensorNaoEncontradoException or
                MonitoramentoNaoEncontradoException => (StatusCodes.Status404NotFound, exception.Message),
            EmailDuplicadoException => (StatusCodes.Status409Conflict, exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };
}
