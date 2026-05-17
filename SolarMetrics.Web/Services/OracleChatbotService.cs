using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarMetrics.Web.Configuration;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Services;

public sealed class OracleChatbotService(
    HttpClient httpClient,
    IOptions<ChatbotSettings> chatbotOptions,
    ILogger<OracleChatbotService> logger) : IOracleChatbotService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ChatbotSettings _settings = chatbotOptions.Value;

    public async Task<ChatbotAskResult> AskAsync(string? question, CancellationToken cancellationToken = default)
    {
        var trimmed = (question ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(trimmed))
            return ChatbotAskResult.Fail("Digite uma pergunta sobre o sistema SolarMetrics.");

        var url = (_settings.OrdsAskUrl ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(url))
        {
            return ChatbotAskResult.Fail(
                "Chatbot não configurado. Defina Chatbot:OrdsAskUrl em appsettings ou Chatbot__OrdsAskUrl no App Service.");
        }

        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                url,
                new { question = trimmed },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("ORDS chatbot retornou HTTP {StatusCode}", (int)response.StatusCode);
                return ChatbotAskResult.Fail(
                    $"Não foi possível consultar o assistente Oracle (HTTP {(int)response.StatusCode}). Tente novamente em instantes.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<OrdsChatbotResponse>(
                stream,
                JsonOptions,
                cancellationToken);

            if (payload is null || string.IsNullOrWhiteSpace(payload.Answer))
            {
                logger.LogWarning("Resposta ORDS sem campo answer");
                return ChatbotAskResult.Fail("Resposta inválida do assistente Oracle. Tente reformular a pergunta.");
            }

            return ChatbotAskResult.Ok(
                payload.Question ?? trimmed,
                payload.Answer,
                payload.Source);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Timeout ao consultar ORDS chatbot");
            return ChatbotAskResult.Fail(
                "A consulta à IA Oracle demorou demais. Tente uma pergunta mais simples ou aguarde e tente novamente.");
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Falha HTTP ao consultar ORDS chatbot");
            return ChatbotAskResult.Fail(
                "Não foi possível contactar o serviço Oracle Select AI. Verifique a conectividade e Chatbot:OrdsAskUrl.");
        }
    }

    private sealed class OrdsChatbotResponse
    {
        [JsonPropertyName("question")]
        public string? Question { get; set; }

        [JsonPropertyName("answer")]
        public string? Answer { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }
}
