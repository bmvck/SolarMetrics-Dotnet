using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using SolarMetrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarMetrics.Configuration;
using SolarMetrics.Infrastructure.Mongo;
using SolarMetrics.Infrastructure.Persistence.Repositories;
using SolarMetrics.Middleware;
using SolarMetrics.UseCase;
using SolarMetrics.Utils;

namespace SolarMetrics.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddSolarMetrics(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

        var swaggerConfig = builder.Configuration.GetSection("Swagger").Get<SwaggerConfig>() ?? new SwaggerConfig
        {
            Title = "SolarMetrics API",
            Description = "API SolarMetrics",
            Contact = new OpenApiContact(),
            Servers = []
        };
        swaggerConfig.Servers ??= [];
        swaggerConfig.Contact ??= new OpenApiContact();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = swaggerConfig.Title,
                Version = "v1",
                Description = swaggerConfig.Description,
                Contact = swaggerConfig.Contact
            });

            swagger.EnableAnnotations();
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Bearer. Em desenvolvimento use POST /auth/token."
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            foreach (var server in swaggerConfig.Servers)
            {
                swagger.AddServer(new OpenApiServer
                {
                    Url = server.Url,
                    Description = server.Name
                });
            }
        });

        var oracleCs = builder.Configuration.GetConnectionString("OracleDb");
        builder.Services.AddDbContext<SolarMetricsContext>(options => options.UseOracle(oracleCs));

        var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        if (string.IsNullOrWhiteSpace(jwt.Key))
            throw new InvalidOperationException(
                $"Configure '{JwtSettings.SectionName}:Key' (mín. 32 caracteres) via appsettings ou User Secrets.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
        builder.Services.AddScoped<IClienteUseCase, ClienteUseCase>();
        builder.Services.AddScoped<ISistemaRepository, SistemaRepository>();
        builder.Services.AddScoped<ISistemaUseCase, SistemaUseCase>();
        builder.Services.AddScoped<IPainelSolarRepository, PainelSolarRepository>();
        builder.Services.AddScoped<IPainelSolarUseCase, PainelSolarUseCase>();
        builder.Services.AddScoped<ISensorRepository, SensorRepository>();
        builder.Services.AddScoped<ISensorUseCase, SensorUseCase>();
        builder.Services.AddScoped<IMonitoramentoRepository, MonitoramentoRepository>();
        builder.Services.AddScoped<IMonitoramentoUseCase, MonitoramentoUseCase>();

        builder.Services.AddSolarMetricsMongoDb(builder.Configuration, builder.Environment);
        builder.Services.AddSolarMetricsObservability(builder.Configuration, builder.Environment);

        return builder;
    }

    public static WebApplication UseSolarMetricsPipeline(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.UseSwagger();
            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1/swagger.json", "SolarMetrics.API v1");
                ui.RoutePrefix = "swagger";
            });
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapSolarMetricsHealthChecks();

        return app;
    }
}
