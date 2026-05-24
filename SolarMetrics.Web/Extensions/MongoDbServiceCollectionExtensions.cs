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
            services.AddSingleton<IMongoClient>(_ => CreateMongoClient(settings.ConnectionString));
            services.AddScoped<IChatbotInteractionRepository, MongoChatbotInteractionRepository>();
            services.AddHostedService<MongoDbIndexInitializerHostedService>();
        }
        else
        {
            services.AddScoped<IChatbotInteractionRepository, NullChatbotInteractionRepository>();
        }

        return services;
    }

    private static MongoClient CreateMongoClient(string connectionString)
    {
        var mongoSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        mongoSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
        return new MongoClient(mongoSettings);
    }
}
