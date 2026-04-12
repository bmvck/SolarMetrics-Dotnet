using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SolarMetrics.Web.Configuration;

namespace SolarMetrics.Web.Services;

public sealed class AdminTokenAcquisitionService(
    IHttpClientFactory httpClientFactory,
    IOptions<ApiSettings> apiOptions,
    IHostEnvironment environment,
    LocalJwtIssuer localJwtIssuer)
{
    private readonly ApiSettings _api = apiOptions.Value;

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var baseUrl = (_api.BaseUrl ?? string.Empty).TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
            return environment.IsDevelopment() ? localJwtIssuer.CreateAccessToken() : null;

        try
        {
            var client = httpClientFactory.CreateClient(nameof(AdminTokenAcquisitionService));
            client.Timeout = TimeSpan.FromSeconds(10);
            using var response = await client.PostAsync($"{baseUrl}/auth/token", null, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return environment.IsDevelopment() ? localJwtIssuer.CreateAccessToken() : null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (doc.RootElement.TryGetProperty("access_token", out var tokenEl))
            {
                var token = tokenEl.GetString();
                if (!string.IsNullOrWhiteSpace(token))
                    return token;
            }

            return environment.IsDevelopment() ? localJwtIssuer.CreateAccessToken() : null;
        }
        catch (HttpRequestException)
        {
            return environment.IsDevelopment() ? localJwtIssuer.CreateAccessToken() : null;
        }
        catch (TaskCanceledException)
        {
            return environment.IsDevelopment() ? localJwtIssuer.CreateAccessToken() : null;
        }
    }
}
