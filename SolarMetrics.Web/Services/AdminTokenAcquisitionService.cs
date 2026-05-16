using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarMetrics.Web.Configuration;

namespace SolarMetrics.Web.Services;

public sealed class AdminTokenAcquisitionService(
    IHttpClientFactory httpClientFactory,
    IOptions<ApiSettings> apiOptions,
    IHostEnvironment environment,
    LocalJwtIssuer localJwtIssuer,
    ILogger<AdminTokenAcquisitionService> logger)
{
    private readonly ApiSettings _api = apiOptions.Value;

    public async Task<TokenAcquisitionResult> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var baseUrl = (_api.BaseUrl ?? string.Empty).TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
        {
            if (environment.IsDevelopment())
                return TokenAcquisitionResult.Success(localJwtIssuer.CreateAccessToken());

            return TokenAcquisitionResult.Failed(
                "Api:BaseUrl não configurada. No App Service defina Api__BaseUrl (ex.: https://solarmetrics-api-rm567164.azurewebsites.net).");
        }

        if (IsLocalhostUrl(baseUrl) && !environment.IsDevelopment())
        {
            logger.LogWarning("Api:BaseUrl aponta para localhost em ambiente não-Development: {BaseUrl}", baseUrl);
            return TokenAcquisitionResult.Failed(
                $"Api:BaseUrl inválida para produção ({baseUrl}). Configure Api__BaseUrl no App Service com a URL pública da API .NET.");
        }

        try
        {
            var client = httpClientFactory.CreateClient(nameof(AdminTokenAcquisitionService));
            client.Timeout = environment.IsDevelopment()
                ? TimeSpan.FromSeconds(10)
                : TimeSpan.FromSeconds(90);

            var tokenUrl = $"{baseUrl}/auth/token";
            using var response = await client.PostAsync(tokenUrl, null, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "POST {TokenUrl} retornou {StatusCode}",
                    tokenUrl,
                    (int)response.StatusCode);

                if (environment.IsDevelopment())
                    return TokenAcquisitionResult.Success(localJwtIssuer.CreateAccessToken());

                var hint = (int)response.StatusCode == 404
                    ? " A API em Production desliga /auth/token — use ASPNETCORE_ENVIRONMENT=Staging na API."
                    : string.Empty;
                return TokenAcquisitionResult.Failed(
                    $"Não foi possível obter o token em {tokenUrl} (HTTP {(int)response.StatusCode}).{hint} Verifique Api__BaseUrl e se a API está em execução.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (doc.RootElement.TryGetProperty("access_token", out var tokenEl))
            {
                var token = tokenEl.GetString();
                if (!string.IsNullOrWhiteSpace(token))
                    return TokenAcquisitionResult.Success(token);
            }

            logger.LogWarning("Resposta de {TokenUrl} sem access_token", tokenUrl);
            if (environment.IsDevelopment())
                return TokenAcquisitionResult.Success(localJwtIssuer.CreateAccessToken());

            return TokenAcquisitionResult.Failed(
                $"Resposta inválida de {tokenUrl}. Verifique Api__BaseUrl (ex.: https://solarmetrics-api-rm567164.azurewebsites.net).");
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Falha HTTP ao obter token de {BaseUrl}/auth/token", baseUrl);
            if (environment.IsDevelopment())
                return TokenAcquisitionResult.Success(localJwtIssuer.CreateAccessToken());

            return TokenAcquisitionResult.Failed(
                $"Não foi possível contactar a API em {baseUrl}. Verifique Api__BaseUrl no App Service e aguarde o cold start da API (pode levar até 1 minuto).");
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "Timeout ao obter token de {BaseUrl}/auth/token", baseUrl);
            if (environment.IsDevelopment())
                return TokenAcquisitionResult.Success(localJwtIssuer.CreateAccessToken());

            return TokenAcquisitionResult.Failed(
                $"Tempo esgotado ao contactar {baseUrl}/auth/token. A API pode estar em cold start — tente novamente em alguns segundos.");
        }
    }

    private static bool IsLocalhostUrl(string baseUrl) =>
        baseUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
        || baseUrl.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase);
}
