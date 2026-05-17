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
[SwaggerTag("CRUD de monitoramentos.")]
public sealed class MonitoramentoController(IMonitoramentoUseCase monitoramentoUseCase) : ControllerBase
{
    private const string CollectionPath = "/Monitoramento";

    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.Created, "Criado", typeof(ApiResource<MonitoramentoResponse>))]
    public async Task<ActionResult<ApiResource<MonitoramentoResponse>>> Post(MonitoramentoDTO dto, CancellationToken cancellationToken)
    {
        var created = await monitoramentoUseCase.CreateAsync(MonitoramentoDTO.ToEntity(dto), cancellationToken);
        var response = HateoasLinkBuilder.ToApiResource(Request, CollectionPath, created.Id, MonitoramentoResponse.ToResponse(created));
        return Created($"{CollectionPath}/{created.Id}", response);
    }

    [HttpPut]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<MonitoramentoResponse>))]
    public async Task<ActionResult<ApiResource<MonitoramentoResponse>>> Put(MonitoramentoDTOUpdate dto, CancellationToken cancellationToken)
    {
        var updated = await monitoramentoUseCase.UpdateAsync(dto.Id, ToEntity(dto), cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, updated.Id, MonitoramentoResponse.ToResponse(updated)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<MonitoramentoResponse>))]
    public async Task<ActionResult<ApiResource<MonitoramentoResponse>>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await monitoramentoUseCase.GetByIdAsync(id, cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, id, MonitoramentoResponse.ToResponse(item)));
    }

    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista", typeof(PagedResource<MonitoramentoResponse>))]
    public async Task<ActionResult<PagedResource<MonitoramentoResponse>>> GetAll([FromQuery] MonitoramentoListQuery query, CancellationToken cancellationToken)
    {
        query.Normalize();
        var (items, total) = await monitoramentoUseCase.GetPagedAsync(query, cancellationToken);
        var dtoItems = items.Select(MonitoramentoResponse.ToResponse).ToList();
        var extra = new Dictionary<string, string?>
        {
            ["sensorId"] = query.SensorId?.ToString(),
            ["periodo"] = query.Periodo,
            ["sortBy"] = query.SortBy,
            ["sortDir"] = query.SortDir
        };
        return Ok(HateoasLinkBuilder.ToPagedResource(Request, CollectionPath, dtoItems, query.Page, query.PageSize, total, extra));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await monitoramentoUseCase.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static Monitoramento ToEntity(MonitoramentoDTOUpdate dto) => new()
    {
        Periodo = dto.Periodo,
        ValorLido = dto.ValorLido,
        MediaLeitura = dto.MediaLeitura,
        MaximaLeitura = dto.MaximaLeitura,
        SensorId = dto.SensorId
    };
}
