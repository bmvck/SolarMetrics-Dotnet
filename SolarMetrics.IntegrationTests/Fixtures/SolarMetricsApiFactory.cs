using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SolarMetrics.IntegrationTests.Fixtures;

public sealed class SolarMetricsApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sqlite;
    private readonly object _dbLock = new();

    public SolarMetricsApiFactory()
    {
        _sqlite = new SqliteConnection("DataSource=solarmetrics_shared;Mode=Memory;Cache=Shared");
        _sqlite.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestKeys.JwtKey,
                ["Jwt:Issuer"] = "SolarMetrics",
                ["Jwt:Audience"] = "SolarMetrics",
                ["ConnectionStrings:OracleDb"] = "unused-in-tests"
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoverRegistro<DbContextOptions<SolarMetricsContext>>(services);
            RemoverRegistro<SolarMetricsContext>(services);

            services.AddDbContext<SolarMetricsContext>(options => options.UseSqlite(_sqlite));
        });
    }

    public new HttpClient CreateClient(WebApplicationFactoryClientOptions? options = null)
    {
        var client = base.CreateClient(options ?? new WebApplicationFactoryClientOptions());
        GarantirSchema();
        return client;
    }

    private void GarantirSchema()
    {
        lock (_dbLock)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SolarMetricsContext>();
            db.Database.EnsureCreated();
        }
    }

    /// <summary>Remove o schema SQLite para isolar cenários entre testes que compartilham a factory.</summary>
    public void LimparBanco()
    {
        lock (_dbLock)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SolarMetricsContext>();
            db.Database.EnsureDeleted();
        }
    }

    private static void RemoverRegistro<T>(IServiceCollection services)
    {
        foreach (var d in services.Where(d => d.ServiceType == typeof(T)).ToList())
            services.Remove(d);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _sqlite.Dispose();
        base.Dispose(disposing);
    }
}
