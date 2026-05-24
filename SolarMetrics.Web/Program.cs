using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SolarMetrics.Web;
using SolarMetrics.Web.Auth;
using SolarMetrics.Web.Configuration;
using SolarMetrics.Web.Extensions;
using SolarMetrics.Web.Repositories;
using SolarMetrics.Web.Services;
using SolarMetrics.Web.UseCase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));
builder.Services.Configure<ChatbotSettings>(builder.Configuration.GetSection(ChatbotSettings.SectionName));
builder.Services.AddSolarMetricsMongoDb(builder.Configuration);

builder.Services.AddDbContext<SolarMetricsContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb"))
);

var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSection.Key))
    throw new InvalidOperationException($"Configure '{JwtSettings.SectionName}:Key' via appsettings ou User Secrets.");

var apiSection = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>() ?? new ApiSettings();
var apiBaseUrl = (apiSection.BaseUrl ?? string.Empty).Trim();
if (builder.Environment.IsProduction())
{
    if (string.IsNullOrEmpty(apiBaseUrl))
        throw new InvalidOperationException(
            $"Em Production configure '{ApiSettings.SectionName}:BaseUrl' (App Service: Api__BaseUrl) com a URL HTTPS da SolarMetrics.API.");

    if (apiBaseUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
        || apiBaseUrl.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            $"Em Production '{ApiSettings.SectionName}:BaseUrl' não pode ser localhost. Defina Api__BaseUrl no App Service (ex.: https://solarmetrics-api-rm567164.azurewebsites.net).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.Issuer,
            ValidAudience = jwtSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue(AdminAuthCookie.Name, out var token))
                    context.Token = token;
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
                context.Response.Redirect("/Account/Login?returnUrl=" + returnUrl);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IOracleChatbotService, OracleChatbotService>((sp, client) =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ChatbotSettings>>().Value;
    var timeout = settings.TimeoutSeconds > 0 ? settings.TimeoutSeconds : 120;
    client.Timeout = TimeSpan.FromSeconds(timeout);
});
builder.Services.AddSingleton<LocalJwtIssuer>();
builder.Services.AddScoped<AdminTokenAcquisitionService>();

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
