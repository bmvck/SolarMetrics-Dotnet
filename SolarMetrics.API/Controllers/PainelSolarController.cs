using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.Common;
using SolarMetrics.DTOs;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.UseCase;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarMetrics.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("CRUD de painéis solares.")]
public sealed class PainelSolarController(IPainelSolarUseCase painelUseCase) : ControllerBase
{
    private const string CollectionPath = "/PainelSolar";

    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.Created, "Criado", typeof(ApiResource<PainelSolarResponse>))]
    public async Task<ActionResult<ApiResource<PainelSolarResponse>>> Post(PainelSolarDTO dto, CancellationToken cancellationToken)
    {
        var created = await painelUseCase.CreateAsync(PainelSolarDTO.ToEntity(dto), cancellationToken);
        var response = HateoasLinkBuilder.ToApiResource(Request, CollectionPath, created.Id, PainelSolarResponse.ToResponse(created));
        return Created($"{CollectionPath}/{created.Id}", response);
    }

    [HttpPut]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<PainelSolarResponse>))]
    public async Task<ActionResult<ApiResource<PainelSolarResponse>>> Put(PainelSolarDTOUpdate dto, CancellationToken cancellationToken)
    {
        var updated = await painelUseCase.UpdateAsync(dto.Id, ToEntity(dto), cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, updated.Id, PainelSolarResponse.ToResponse(updated)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<PainelSolarResponse>))]
    public async Task<ActionResult<ApiResource<PainelSolarResponse>>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await painelUseCase.GetByIdAsync(id, cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, id, PainelSolarResponse.ToResponse(item)));
    }

    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista", typeof(PagedResource<PainelSolarResponse>))]
    public async Task<ActionResult<PagedResource<PainelSolarResponse>>> GetAll([FromQuery] PainelSolarListQuery query, CancellationToken cancellationToken)
    {
        query.Normalize();
        var (items, total) = await painelUseCase.GetPagedAsync(query, cancellationToken);
        var dtoItems = items.Select(PainelSolarResponse.ToResponse).ToList();
        var extra = new Dictionary<string, string?>
        {
            ["modelo"] = query.Modelo,
            ["fabricante"] = query.Fabricante,
            ["sistemaId"] = query.SistemaId?.ToString(),
            ["sortBy"] = query.SortBy,
            ["sortDir"] = query.SortDir
        };
        return Ok(HateoasLinkBuilder.ToPagedResource(Request, CollectionPath, dtoItems, query.Page, query.PageSize, total, extra));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await painelUseCase.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static PainelSolar ToEntity(PainelSolarDTOUpdate dto) => new()
    {
        Modelo = dto.Modelo,
        Fabricante = dto.Fabricante,
        PotenciaMaxima = dto.PotenciaMaxima,
        DataFabricacao = dto.DataFabricacao,
        Eficiencia = dto.Eficiencia,
        SistemaId = dto.SistemaId
    };
}
