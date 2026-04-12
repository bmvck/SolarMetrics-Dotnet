using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SolarMetrics.Exceptions;
using SolarMetrics.Web.Areas.Admin.ViewModels;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.Repositories;
using SolarMetrics.Web.UseCase;

namespace SolarMetrics.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public sealed class SistemaController(ISistemaUseCase sistemaUseCase, IClienteRepository clienteRepository) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var list = await sistemaUseCase.GetAllAsync(cancellationToken);
        return View(list);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var sistema = await sistemaUseCase.GetByIdAsync(id, cancellationToken);
            return View(sistema);
        }
        catch (SistemaNaoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await FillClienteSelectAsync(cancellationToken);
        return View(new SistemaFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SistemaFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillClienteSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var entity = new Sistema
            {
                NomeInstalacao = vm.NomeInstalacao,
                DataInstalacao = vm.DataInstalacao,
                PotenciaTotal = vm.PotenciaTotal,
                Status = vm.Status,
                ClienteId = vm.ClienteId
            };
            await sistemaUseCase.CreateAsync(entity, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (ClienteNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.ClienteId), "Cliente inválido.");
            await FillClienteSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var sistema = await sistemaUseCase.GetByIdAsync(id, cancellationToken);
            await FillClienteSelectAsync(cancellationToken);
            var vm = new SistemaFormViewModel
            {
                Id = sistema.Id,
                NomeInstalacao = sistema.NomeInstalacao,
                DataInstalacao = sistema.DataInstalacao,
                PotenciaTotal = sistema.PotenciaTotal,
                Status = sistema.Status,
                ClienteId = sistema.ClienteId
            };
            return View(vm);
        }
        catch (SistemaNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SistemaFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillClienteSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var dados = new Sistema
            {
                NomeInstalacao = vm.NomeInstalacao,
                DataInstalacao = vm.DataInstalacao,
                PotenciaTotal = vm.PotenciaTotal,
                Status = vm.Status,
                ClienteId = vm.ClienteId
            };
            await sistemaUseCase.UpdateAsync(id, dados, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SistemaNaoEncontradoException)
        {
            return NotFound();
        }
        catch (ClienteNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.ClienteId), "Cliente inválido.");
            await FillClienteSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var sistema = await sistemaUseCase.GetByIdAsync(id, cancellationToken);
            return View(sistema);
        }
        catch (SistemaNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await sistemaUseCase.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SistemaNaoEncontradoException)
        {
            return NotFound();
        }
    }

    private async Task FillClienteSelectAsync(CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.GetAllAsync();
        ViewBag.ClienteId = clientes.Select(c => new SelectListItem(c.Nome, c.Id.ToString())).ToList();
    }
}
