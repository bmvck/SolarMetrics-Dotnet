using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SolarMetrics.IntegrationTests.Fixtures;
using Xunit;

namespace SolarMetrics.IntegrationTests;

[Collection("ApiIntegration")]
public sealed class ClienteControllerIntegrationTests
{
    private readonly SolarMetricsApiFactory _factory;

    public ClienteControllerIntegrationTests(SolarMetricsApiFactory factory)
    {
        _factory = factory;
    }

    private static async Task AutenticarAsync(HttpClient client)
    {
        var tokenRes = await client.PostAsync("/auth/token", null);
        tokenRes.EnsureSuccessStatusCode();
        await using var stream = await tokenRes.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var token = doc.RootElement.GetProperty("access_token").GetString();
        Assert.NotNull(token);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetAll_SemToken_Retorna401Unauthorized()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();

        var res = await client.GetAsync("/Cliente");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task GetAll_ComToken_ListaVazia_Retorna200ComEnvelopePaginado()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        await AutenticarAsync(client);

        var res = await client.GetAsync("/Cliente?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        await using var stream = await res.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        Assert.True(doc.RootElement.TryGetProperty("_links", out _));
        Assert.Equal(0, doc.RootElement.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task GetCliente_ComToken_RetornaHateoasLinks()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        await AutenticarAsync(client);
        var email = $"hateoas_{Guid.NewGuid():N}@test.com";
        var body = new { nome = "HATEOAS", email, telefone = "11988887777", tipoUsuario = "ADMIN" };
        var create = await client.PostAsJsonAsync("/Cliente", body);
        create.EnsureSuccessStatusCode();
        using var createDoc = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var id = createDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var res = await client.GetAsync($"/Cliente/{id}");

        res.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var links = doc.RootElement.GetProperty("_links");
        Assert.True(links.GetArrayLength() >= 3);
    }

    [Fact]
    public async Task PostCliente_ComTokenValido_Retorna201Created()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        await AutenticarAsync(client);
        var email = $"criado_{Guid.NewGuid():N}@test.com";
        var body = new
        {
            nome = "Integração",
            email,
            telefone = "11988887777",
            tipoUsuario = "ADMIN"
        };

        var res = await client.PostAsJsonAsync("/Cliente", body);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task GetCliente_IdInexistente_ComToken_Retorna404NotFound()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        await AutenticarAsync(client);
        var id = Guid.NewGuid();

        var res = await client.GetAsync($"/Cliente/{id}");

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task PostCliente_EmailDuplicado_ComToken_Retorna409Conflict()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();
        await AutenticarAsync(client);
        var email = $"dup_{Guid.NewGuid():N}@test.com";
        var body = new
        {
            nome = "Um",
            email,
            telefone = "11988887777",
            tipoUsuario = "ADMIN"
        };
        var first = await client.PostAsJsonAsync("/Cliente", body);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/Cliente", body);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task HealthLive_SemToken_Retorna200Ok()
    {
        _factory.LimparBanco();
        var client = _factory.CreateClient();

        var res = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
