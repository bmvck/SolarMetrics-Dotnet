using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.DTOs;
using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.UseCase;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarMetrics.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("Manipulação dos cadastros dos nosso clientes (CRUD). " +
            "Permite criar, atualizar, buscar e deletar os cadastros no sistema.")]

public class ClienteController : ControllerBase
{
    private readonly IClienteUseCase  _clienteUseCase;

    public ClienteController(IClienteUseCase clienteUseCase)
    {
        _clienteUseCase = clienteUseCase;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Criar cadastro do cliente", Description = "Adiciona um novo cadastro de cliente na aplicação")]
    [SwaggerResponse((int)HttpStatusCode.Created, "Cadastro criado com êxito",  typeof(ClienteResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Erro na requisição")]
    [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflito nas informações de cadastro")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Sem autorização")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Serviço indisponível")]
    public async Task<ActionResult<Cliente>> PostCliente(ClienteDTO clienteDto)
    {
        try
        {
            var clienteCriado = await _clienteUseCase.CreateAsync(ClienteDTO.ToEntity(clienteDto));
            return Created("", ClienteResponse.ToResponse(clienteCriado));
        }
        catch (EmailDuplicadoException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    
    [HttpPut]
    [SwaggerOperation(Summary = "Atualizar cadastro do cliente", Description = "Altera todo o cadastro do cliente informado")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Cadastro atualizado com êxito",  typeof(ClienteResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Erro na requisição")]
    [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflito nas informações de cadastro")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Cliente não encontrado")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Sem autorização")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Serviço indisponível")]
    public async Task<ActionResult<Cliente>> PutCliente(ClienteDTOUpdate clienteDtoUpdate)
    {
        try
        {
            var clienteAtualizado = await _clienteUseCase.UpdateAsync(ClienteDTOUpdate.ToEntity(clienteDtoUpdate));
            return Ok(ClienteResponse.ToResponse(clienteAtualizado));
        }
        catch (EmailDuplicadoException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Buscar cadastro do cliente", Description = "Encontra o cadastro de um cliente especifico pelo ID")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Cadastro do cliente é retornado",  typeof(ClienteResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Erro na requisição")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Cliente não encontrado")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Sem autorização")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Serviço indisponível")]
    public async Task<ActionResult<Cliente>> GetCliente(Guid id)
    {
        try
        {
            var cliente = await _clienteUseCase.GetById(id);
            return Ok(ClienteResponse.ToResponse(cliente));
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Buscar todos clientes", Description = "Traz todos os clientes cadastrados atualmente na aplicação")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lista de clientes retornada",  typeof(List<ClienteResponse>))]
    [SwaggerResponse((int)HttpStatusCode.NoContent, "Sem conteúdo")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Sem autorização")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Serviço indisponível")]
    public async Task<ActionResult<Cliente>> GetAll()
    {
        List<Cliente> clientes = await _clienteUseCase.GetAllAsync();
        if (!clientes.Any())
        {
            return NoContent();
        }
        var clientesDto = clientes.Select(ClienteResponse.ToResponse).ToList();
        return Ok(clientesDto);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletar cadastro do cliente", Description = "Apaga o cadastro de um cliente na aplicação")]
    [SwaggerResponse((int)HttpStatusCode.NoContent, "Sem conteúdo")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Erro na requisição")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Cliente não encontrado")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Sem autorização")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Serviço indisponível")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _clienteUseCase.DeleteAsync(id);
            return NoContent();
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}