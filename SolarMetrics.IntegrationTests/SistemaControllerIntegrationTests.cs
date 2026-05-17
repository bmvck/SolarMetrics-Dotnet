using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SolarMetrics.IntegrationTests.Fixtures;
using Xunit;

namespace SolarMetrics.IntegrationTests;

[Collection("ApiIntegration")]
public sealed class SistemaControllerIntegrationTests
{
    private readonly SolarMetricsApiFactory _factory;

    public SistemaControllerIntegrationTests(SolarMetricsApiFactory factory) => _factory = factory;

    private static async Task<string> ObterTokenAsync(HttpClient client)
    {
        var tokenRes = await client.PostAsync("/auth/token", null);
        tokenRes.EnsureSuccessStatusCode();
        await using var stream = await tokenRes.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }

    [Fact]
    public async Task CrudSistema_ComClienteExistente_FluxoCompleto()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        var token = await ObterTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var email = $"sistema_{Guid.NewGuid():N}@test.com";
        var clienteRes = await client.PostAsJsonAsync("/Cliente", new
        {
            nome = "Cliente Sistema",
            email,
            telefone = "11999998888",
            tipoUsuario = "ADMIN"
        });
        clienteRes.EnsureSuccessStatusCode();
        using var clienteDoc = JsonDocument.Parse(await clienteRes.Content.ReadAsStringAsync());
        var clienteId = clienteDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var createRes = await client.PostAsJsonAsync("/Sistema", new
        {
            nomeInstalacao = "Instalação Teste",
            dataInstalacao = DateTime.UtcNow,
            potenciaTotal = 5000,
            status = "ATIVO",
            clienteId
        });
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        using var createDoc = JsonDocument.Parse(await createRes.Content.ReadAsStringAsync());
        var sistemaId = createDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var getRes = await client.GetAsync($"/Sistema/{sistemaId}");
        getRes.EnsureSuccessStatusCode();

        var updateRes = await client.PutAsJsonAsync("/Sistema", new
        {
            id = sistemaId,
            nomeInstalacao = "Instalação Atualizada",
            dataInstalacao = DateTime.UtcNow,
            potenciaTotal = 6000,
            status = "ATIVO",
            clienteId
        });
        updateRes.EnsureSuccessStatusCode();

        var deleteRes = await client.DeleteAsync($"/Sistema/{sistemaId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);
    }
}
