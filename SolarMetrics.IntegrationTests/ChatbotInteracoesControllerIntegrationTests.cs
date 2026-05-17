using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using SolarMetrics.IntegrationTests.Fixtures;
using Xunit;

namespace SolarMetrics.IntegrationTests;

[Collection("ApiIntegration")]
public sealed class ChatbotInteracoesControllerIntegrationTests
{
    private readonly SolarMetricsApiFactory _factory;

    public ChatbotInteracoesControllerIntegrationTests(SolarMetricsApiFactory factory) => _factory = factory;

    [Fact]
    public async Task GetAll_ComToken_MongoDesabilitado_Retorna503()
    {
        var client = _factory.CreateClient();
        var tokenRes = await client.PostAsync("/auth/token", null);
        tokenRes.EnsureSuccessStatusCode();
        await using var stream = await tokenRes.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var token = doc.RootElement.GetProperty("access_token").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.GetAsync("/ChatbotInteracoes?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, res.StatusCode);
    }
}
