using Microsoft.Extensions.Options;
using SolarMetrics.Web.Configuration;

namespace SolarMetrics.Web.Services;

/// <summary>
/// Cria índices MongoDB em background após o Kestrel subir, sem bloquear o startup.
/// </summary>
internal sealed class MongoDbIndexInitializerHostedService(
    IServiceProvider services,
    IOptions<MongoDbSettings> options,
    ILogger<MongoDbIndexInitializerHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.Enabled || string.IsNullOrWhiteSpace(settings.ConnectionString))
            return Task.CompletedTask;

        _ = InitializeIndexesAsync(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task InitializeIndexesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IChatbotInteractionRepository>();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(15));
            await repository.EnsureIndexesAsync(timeoutCts.Token).ConfigureAwait(false);
            logger.LogInformation("Índices MongoDB garantidos em background.");
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Timeout ao criar índices MongoDB em background.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Não foi possível criar índices MongoDB em background.");
        }
    }
}
