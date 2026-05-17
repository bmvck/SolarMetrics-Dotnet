using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolarMetrics.Web.Configuration;
using SolarMetrics.Web.Services;

namespace SolarMetrics.Web.Extensions;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddSolarMetricsMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));

        var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>() ?? new MongoDbSettings();
        if (settings.Enabled && !string.IsNullOrWhiteSpace(settings.ConnectionString))
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
            await repository.EnsureIndexesAsync();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("MongoDb");
            logger.LogWarning(ex, "Não foi possível criar índices MongoDB na inicialização. Verifique MongoDb:ConnectionString.");
        }
    }
}
