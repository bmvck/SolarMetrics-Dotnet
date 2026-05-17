using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SolarMetrics.Web.Configuration;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.Services;

namespace SolarMetrics.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public sealed class ChatbotController(
    IOracleChatbotService chatbot,
    IChatbotInteractionRepository interactions,
    IOptions<MongoDbSettings> mongoOptions,
    ILogger<ChatbotController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Assistente IA";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> History(int page = 1, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Histórico do assistente IA";

        var pageSize = mongoOptions.Value.HistoryPageSize > 0 ? mongoOptions.Value.HistoryPageSize : 50;
        var (documents, totalCount) = await interactions.ListRecentAsync(page, pageSize, cancellationToken);

        var model = new ChatbotHistoryViewModel
        {
            Page = Math.Max(1, page),
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = documents.Select(ChatbotInteractionMapper.ToListItem).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask([FromBody] ChatbotAskRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await chatbot.AskAsync(request.Question, cancellationToken);
        stopwatch.Stop();

        await TryPersistInteractionAsync(request.Question, result, stopwatch.ElapsedMilliseconds, cancellationToken);

        return Json(result);
    }

    private async Task TryPersistInteractionAsync(
        string? questionFromRequest,
        ChatbotAskResult result,
        long durationMs,
        CancellationToken cancellationToken)
    {
        try
        {
            var document = ChatbotInteractionMapper.FromAskResult(
                result,
                questionFromRequest,
                durationMs,
                User,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            await interactions.InsertAsync(document, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao registrar interação do chatbot no MongoDB");
        }
    }
}
