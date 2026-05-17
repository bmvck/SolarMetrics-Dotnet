using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.Common;
using SolarMetrics.DTOs;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.UseCase;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarMetrics.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("CRUD de clientes com paginação, filtros, ordenação e HATEOAS.")]
public class ClienteController(IClienteUseCase clienteUseCase) : ControllerBase
{
    private const string CollectionPath = "/Cliente";

    [HttpPost]
    [SwaggerOperation(Summary = "Criar cliente")]
    [SwaggerResponse((int)HttpStatusCode.Created, "Criado", typeof(ApiResource<ClienteResponse>))]
    public async Task<ActionResult<ApiResource<ClienteResponse>>> PostCliente(ClienteDTO clienteDto, CancellationToken cancellationToken)
    {
        var clienteCriado = await clienteUseCase.CreateAsync(ClienteDTO.ToEntity(clienteDto), cancellationToken);
        var response = HateoasLinkBuilder.ToApiResource(Request, CollectionPath, clienteCriado.Id, ClienteResponse.ToResponse(clienteCriado));
        return Created($"{CollectionPath}/{clienteCriado.Id}", response);
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Atualizar cliente")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Atualizado", typeof(ApiResource<ClienteResponse>))]
    public async Task<ActionResult<ApiResource<ClienteResponse>>> PutCliente(ClienteDTOUpdate clienteDtoUpdate, CancellationToken cancellationToken)
    {
        var clienteAtualizado = await clienteUseCase.UpdateAsync(ClienteDTOUpdate.ToEntity(clienteDtoUpdate), cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, clienteAtualizado.Id, ClienteResponse.ToResponse(clienteAtualizado)));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Buscar cliente por id")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Encontrado", typeof(ApiResource<ClienteResponse>))]
    public async Task<ActionResult<ApiResource<ClienteResponse>>> GetCliente(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clienteUseCase.GetById(id, cancellationToken);
        return Ok(HateoasLinkBuilder.ToApiResource(Request, CollectionPath, id, ClienteResponse.ToResponse(cliente)));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar clientes paginado")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista paginada", typeof(PagedResource<ClienteResponse>))]
    public async Task<ActionResult<PagedResource<ClienteResponse>>> GetAll([FromQuery] ClienteListQuery query, CancellationToken cancellationToken)
    {
        query.Normalize();
        var (items, total) = await clienteUseCase.GetPagedAsync(query, cancellationToken);
        var dtoItems = items.Select(ClienteResponse.ToResponse).ToList();
        var extra = new Dictionary<string, string?>
        {
            ["nome"] = query.Nome,
            ["email"] = query.Email,
            ["tipoUsuario"] = query.TipoUsuario,
            ["sortBy"] = query.SortBy,
            ["sortDir"] = query.SortDir
        };
        return Ok(HateoasLinkBuilder.ToPagedResource(Request, CollectionPath, dtoItems, query.Page, query.PageSize, total, extra));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Remover cliente")]
    [SwaggerResponse((int)HttpStatusCode.NoContent, "Removido")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await clienteUseCase.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
