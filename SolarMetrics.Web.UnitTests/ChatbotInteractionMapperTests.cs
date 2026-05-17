using System.Security.Claims;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.Services;
using Xunit;

namespace SolarMetrics.Web.UnitTests;

public sealed class ChatbotInteractionMapperTests
{
    [Fact]
    public void FromAskResult_Sucesso_MapeiaCamposPrincipais()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim(ClaimTypes.Name, "api-user")
        }, "Test"));

        var result = ChatbotAskResult.Ok("Quantos clientes?", "Existem 5 clientes.", "Oracle Select AI");

        var document = ChatbotInteractionMapper.FromAskResult(
            result,
            "Quantos clientes?",
            1500,
            user,
            "127.0.0.1",
            "Mozilla/5.0 Test");

        Assert.True(document.Success);
        Assert.Equal("user-123", document.UserId);
        Assert.Equal("api-user", document.UserName);
        Assert.Equal("Quantos clientes?", document.Question);
        Assert.Equal("Existem 5 clientes.", document.Answer);
        Assert.Equal("Oracle Select AI", document.Source);
        Assert.Equal(1500, document.DurationMs);
        Assert.Equal("127.0.0.1", document.ClientIp);
        Assert.Equal("Mozilla/5.0 Test", document.UserAgent);
        Assert.Null(document.ErrorMessage);
    }

    [Fact]
    public void FromAskResult_Falha_MapeiaErroSemResposta()
    {
        var result = ChatbotAskResult.Fail("Serviço indisponível");

        var document = ChatbotInteractionMapper.FromAskResult(
            result,
            "Pergunta teste",
            200,
            null,
            null,
            null);

        Assert.False(document.Success);
        Assert.Equal("Pergunta teste", document.Question);
        Assert.Null(document.Answer);
        Assert.Equal("Serviço indisponível", document.ErrorMessage);
    }

    [Fact]
    public void FromAskResult_UserAgentLongo_TruncaEm512Caracteres()
    {
        var longAgent = new string('x', 600);
        var result = ChatbotAskResult.Ok("q", "a", null);

        var document = ChatbotInteractionMapper.FromAskResult(
            result, "q", 1, null, null, longAgent);

        Assert.NotNull(document.UserAgent);
        Assert.Equal(512, document.UserAgent!.Length);
    }

    [Fact]
    public void ToListItem_MapeiaIdEStatus()
    {
        var document = new ChatbotInteractionDocument
        {
            Id = "507f1f77bcf86cd799439011",
            CreatedAtUtc = new DateTime(2026, 5, 17, 12, 0, 0, DateTimeKind.Utc),
            UserName = "admin",
            Question = "Teste",
            Answer = "Resposta",
            Source = "Oracle",
            Success = true,
            DurationMs = 100
        };

        var item = ChatbotInteractionMapper.ToListItem(document);

        Assert.Equal(document.Id, item.Id);
        Assert.Equal("admin", item.UserName);
        Assert.True(item.Success);
        Assert.Equal(100, item.DurationMs);
    }
}
