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
[SwaggerTag("CRUD de sistemas solares.")]
public sealed class SistemaController(ISistemaUseCase sistemaUseCase) : ControllerBase
{
    private const string CollectionPath = "/Sistema";

    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.Created, "Criado", typeof(ApiResource<SistemaResponse>))]
    public async Task<ActionResult<ApiResource<SistemaResponse>>> Post(SistemaDTO dto, CancellationToken cancellationToken)
    {
        var created = await sistemaUseCase.CreateAsync(SistemaDTO.ToEntity(dto), cancellationToken);
        var response = HateoasLinkBuilder.ToApiResource(Request, CollectionPath, created.Id, SistemaResponse.ToResponse(created));
        return Created($"{CollectionPath}/{created.Id}", response);
    }

    [HttpPut]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<SistemaResponse>))]
    public async Task<ActionResult<ApiResource<SistemaResponse>>> Put(SistemaDTOUpdate dto, CancellationToken cancellationToken)
    {
        var updated = await sistemaUseCase.UpdateAsync(dto.Id, ToEntity(dto), cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, updated.Id, SistemaResponse.ToResponse(updated)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<SistemaResponse>))]
    public async Task<ActionResult<ApiResource<SistemaResponse>>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await sistemaUseCase.GetByIdAsync(id, cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, id, SistemaResponse.ToResponse(item)));
    }

    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista", typeof(PagedResource<SistemaResponse>))]
    public async Task<ActionResult<PagedResource<SistemaResponse>>> GetAll([FromQuery] SistemaListQuery query, CancellationToken cancellationToken)
    {
        query.Normalize();
        var (items, total) = await sistemaUseCase.GetPagedAsync(query, cancellationToken);
        var dtoItems = items.Select(SistemaResponse.ToResponse).ToList();
        var extra = new Dictionary<string, string?>
        {
            ["nomeInstalacao"] = query.NomeInstalacao,
            ["status"] = query.Status,
            ["clienteId"] = query.ClienteId?.ToString(),
            ["sortBy"] = query.SortBy,
            ["sortDir"] = query.SortDir
        };
        return Ok(HateoasLinkBuilder.ToPagedResource(Request, CollectionPath, dtoItems, query.Page, query.PageSize, total, extra));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sistemaUseCase.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static Sistema ToEntity(SistemaDTOUpdate dto) => new()
    {
        NomeInstalacao = dto.NomeInstalacao,
        DataInstalacao = dto.DataInstalacao,
        PotenciaTotal = dto.PotenciaTotal,
        Status = dto.Status,
        ClienteId = dto.ClienteId
    };
}
