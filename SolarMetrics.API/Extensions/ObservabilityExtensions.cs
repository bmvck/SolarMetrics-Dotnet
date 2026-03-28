using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SolarMetrics;
using SolarMetrics.Configuration;

namespace SolarMetrics.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddSolarMetricsObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var healthSettings = configuration.GetSection(HealthChecksSettings.SectionName).Get<HealthChecksSettings>()
                             ?? new HealthChecksSettings();

        var health = services.AddHealthChecks()
            .AddDbContextCheck<SolarMetricsContext>("oracle_ef", tags: ["ready", "db"]);

        if (!environment.IsEnvironment("Test") &&
            Uri.TryCreate(healthSettings.ExternalUrl, UriKind.Absolute, out var uri))
        {
            var timeout = TimeSpan.FromSeconds(Math.Clamp(healthSettings.ExternalTimeoutSeconds, 1, 60));
            health.AddAsyncCheck(
                "external_http",
                async ct =>
                {
                    try
                    {
                        using var client = new HttpClient { Timeout = timeout };
                        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);
                        return response.IsSuccessStatusCode
                            ? HealthCheckResult.Healthy()
                            : HealthCheckResult.Degraded($"HTTP {(int)response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        return HealthCheckResult.Unhealthy("Falha ao contatar URL externa.", ex);
                    }
                },
                tags: ["ready", "external"]);
        }

        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(
                serviceName: "SolarMetrics.API",
                serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "1.0"))
            .WithTracing(tb => tb
                .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                .AddHttpClientInstrumentation()
                .AddSource("SolarMetrics.API")
                .AddConsoleExporter())
            .WithMetrics(mb => mb
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter());

        return services;
    }

    public static WebApplication MapSolarMetricsHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("ready")
        }).AllowAnonymous();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        }).AllowAnonymous();

        return app;
    }
}
