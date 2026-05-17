using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.Common;
using SolarMetrics.DTOs;
using SolarMetrics.Infrastructure.Mongo;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarMetrics.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("Consulta paginada das interações do chatbot (MongoDB). Somente leitura.")]
public sealed class ChatbotInteracoesController(IChatbotInteractionRepository repository) : ControllerBase
{
    private const string CollectionPath = "/ChatbotInteracoes";

    [HttpGet]
    [SwaggerOperation(Summary = "Listar interações do chatbot")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista paginada", typeof(PagedResource<ChatbotInteracaoResponse>))]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "MongoDB desabilitado")]
    public async Task<ActionResult<PagedResource<ChatbotInteracaoResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (!repository.IsEnabled)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "MongoDB não está habilitado. Configure MongoDb:Enabled e ConnectionString." });

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var (items, total) = await repository.ListRecentAsync(page, pageSize, cancellationToken);
        var dtoItems = items.Select(ChatbotInteracaoResponse.FromDocument).ToList();
        var totalCount = (int)Math.Min(total, int.MaxValue);
        var extra = new Dictionary<string, string?>();

        return Ok(HateoasLinkBuilder.ToPagedResource(Request, CollectionPath, dtoItems, page, pageSize, totalCount, extra));
    }
}
