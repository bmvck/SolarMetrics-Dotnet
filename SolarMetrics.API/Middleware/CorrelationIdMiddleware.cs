using Serilog.Context;

namespace SolarMetrics.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(id))
            id = Guid.NewGuid().ToString("N");

        context.Response.Headers[HeaderName] = id;
        context.Items[ItemKey] = id;

        using (LogContext.PushProperty("CorrelationId", id))
            await next(context);
    }
}
