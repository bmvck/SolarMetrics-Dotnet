using Serilog;
using SolarMetrics.Extensions;

namespace SolarMetrics;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((ctx, services, cfg) =>
            {
                cfg.ReadFrom.Configuration(ctx.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "SolarMetrics.API")
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:O}] [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: "logs/solarmetrics-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14);
            });

            builder.AddSolarMetrics();
            var app = builder.Build();
            await app.EnsureMongoDbIndexesAsync();
            app.UseSolarMetricsPipeline();
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
