using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.Exceptions;
using SolarMetrics.Web.UseCase;
using SolarMetrics.Web.ViewModels;

namespace SolarMetrics.Web.Areas.Vendas.Controllers;

/// <summary>Painel de vendas: apenas cadastro de clientes (fluxo original do projeto).</summary>
[Area("Vendas")]
[Authorize]
public sealed class ClienteController(IClienteUseCase clienteUseCase) : Controller
{
    public async Task<IActionResult> Index()
    {
        var clientes = await clienteUseCase.GetAllAsync();
        var clientesViews = clientes.Select(ClienteViewModel.ToResponse).ToList();
        return View(clientesViews);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var cliente = await clienteUseCase.GetById(id);
            var viewmodel = ClienteViewModel.ToResponse(cliente);
            return View(viewmodel);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteCreateViewModel clienteCreateViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await clienteUseCase.CreateAsync(ClienteCreateViewModel.ToEntity(clienteCreateViewModel));
                return RedirectToAction(nameof(Index));
            }
            catch (EmailDuplicadoException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
            }
        }

        return View(clienteCreateViewModel);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var cliente = await clienteUseCase.GetById(id);
            var viewmodel = ClienteUpdateViewModel.ToResponse(cliente);
            return View(viewmodel);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            var errorViewModel = new ErrorViewModel { Message = ex.Message };
            return View("~/Views/Shared/NotFound.cshtml", errorViewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ClienteUpdateViewModel clienteUpdateViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await clienteUseCase.UpdateAsync(ClienteUpdateViewModel.ToEntity(clienteUpdateViewModel));
                return RedirectToAction(nameof(Index));
            }
            catch (EmailDuplicadoException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
            }
            catch (ClienteNaoEncontradoException ex)
            {
                var errorViewModel = new ErrorViewModel { Message = ex.Message };
                return View("~/Views/Shared/NotFound.cshtml", errorViewModel);
            }
        }

        return View(clienteUpdateViewModel);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var cliente = await clienteUseCase.GetById(id);
            return View(cliente);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            var errorViewModel = new ErrorViewModel { Message = ex.Message };
            return View("~/Views/Shared/NotFound.cshtml", errorViewModel);
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await clienteUseCase.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (ClienteNaoEncontradoException ex)
        {
            var errorViewModel = new ErrorViewModel { Message = ex.Message };
            return View("~/Views/Shared/NotFound.cshtml", errorViewModel);
        }
    }
}
