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
[SwaggerTag("CRUD de sensores.")]
public sealed class SensorController(ISensorUseCase sensorUseCase) : ControllerBase
{
    private const string CollectionPath = "/Sensor";

    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.Created, "Criado", typeof(ApiResource<SensorResponse>))]
    public async Task<ActionResult<ApiResource<SensorResponse>>> Post(SensorDTO dto, CancellationToken cancellationToken)
    {
        var created = await sensorUseCase.CreateAsync(SensorDTO.ToEntity(dto), cancellationToken);
        var response = HateoasLinkBuilder.ToApiResource(Request, CollectionPath, created.Id, SensorResponse.ToResponse(created));
        return Created($"{CollectionPath}/{created.Id}", response);
    }

    [HttpPut]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<SensorResponse>))]
    public async Task<ActionResult<ApiResource<SensorResponse>>> Put(SensorDTOUpdate dto, CancellationToken cancellationToken)
    {
        var updated = await sensorUseCase.UpdateAsync(dto.Id, ToEntity(dto), cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, updated.Id, SensorResponse.ToResponse(updated)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ApiResource<SensorResponse>))]
    public async Task<ActionResult<ApiResource<SensorResponse>>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await sensorUseCase.GetByIdAsync(id, cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, id, SensorResponse.ToResponse(item)));
    }

    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista", typeof(PagedResource<SensorResponse>))]
    public async Task<ActionResult<PagedResource<SensorResponse>>> GetAll([FromQuery] SensorListQuery query, CancellationToken cancellationToken)
    {
        query.Normalize();
        var (items, total) = await sensorUseCase.GetPagedAsync(query, cancellationToken);
        var dtoItems = items.Select(SensorResponse.ToResponse).ToList();
        var extra = new Dictionary<string, string?>
        {
            ["tipo"] = query.Tipo,
            ["status"] = query.Status,
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
        await sensorUseCase.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static Sensor ToEntity(SensorDTOUpdate dto) => new()
    {
        Tipo = dto.Tipo,
        Status = dto.Status,
        Localizacao = dto.Localizacao,
        SistemaId = dto.SistemaId
    };
}
