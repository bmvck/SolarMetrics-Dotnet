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
        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        settings.ConnectTimeout = TimeSpan.FromSeconds(10);
        return new MongoClient(settings);
    }
}
