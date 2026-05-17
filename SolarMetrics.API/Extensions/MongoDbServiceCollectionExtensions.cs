using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolarMetrics.Configuration;
using SolarMetrics.Infrastructure.Mongo;

namespace SolarMetrics.Extensions;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddSolarMetricsMongoDb(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));

        var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>() ?? new MongoDbSettings();
        var useMongo = settings.Enabled
                       && !string.IsNullOrWhiteSpace(settings.ConnectionString)
                       && !environment.IsEnvironment("Test");

        if (useMongo)
        {
            services.AddSingleton<IMongoClient>(_ => new MongoClient(settings.ConnectionString));
            services.AddScoped<IChatbotInteractionRepository, MongoChatbotInteractionRepository>();
        }
        else
        {
            services.AddScoped<IChatbotInteractionRepository, NullChatbotInteractionRepository>();
        }

        return services;
    }

    public static async Task EnsureMongoDbIndexesAsync(this WebApplication app)
    {
        var settings = app.Services.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        if (!settings.Enabled || string.IsNullOrWhiteSpace(settings.ConnectionString))
            return;

        try
        {
            using var scope = app.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IChatbotInteractionRepository>();
            if (!repository.IsEnabled)
                return;
            await repository.EnsureIndexesAsync();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("MongoDb");
            logger.LogWarning(ex, "Não foi possível criar índices MongoDB na inicialização.");
        }
    }
}
