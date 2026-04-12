using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SolarMetrics.Exceptions;
using SolarMetrics.Web.Areas.Admin.ViewModels;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.UseCase;

namespace SolarMetrics.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public sealed class PainelSolarController(ISistemaUseCase sistemaUseCase, IPainelSolarUseCase painelUseCase) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var list = await painelUseCase.GetAllAsync(cancellationToken);
        return View(list);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await painelUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (PainelSolarNaoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await FillSistemaSelectAsync(cancellationToken);
        return View(new PainelSolarFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PainelSolarFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var entity = new PainelSolar
            {
                Modelo = vm.Modelo,
                Fabricante = vm.Fabricante,
                PotenciaMaxima = vm.PotenciaMaxima,
                DataFabricacao = vm.DataFabricacao,
                Eficiencia = vm.Eficiencia,
                SistemaId = vm.SistemaId
            };
            await painelUseCase.CreateAsync(entity, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SistemaNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.SistemaId), "Sistema inválido.");
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var painel = await painelUseCase.GetByIdAsync(id, cancellationToken);
            await FillSistemaSelectAsync(cancellationToken);
            return View(new PainelSolarFormViewModel
            {
                Id = painel.Id,
                Modelo = painel.Modelo,
                Fabricante = painel.Fabricante,
                PotenciaMaxima = painel.PotenciaMaxima,
                DataFabricacao = painel.DataFabricacao,
                Eficiencia = painel.Eficiencia,
                SistemaId = painel.SistemaId
            });
        }
        catch (PainelSolarNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PainelSolarFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var dados = new PainelSolar
            {
                Modelo = vm.Modelo,
                Fabricante = vm.Fabricante,
                PotenciaMaxima = vm.PotenciaMaxima,
                DataFabricacao = vm.DataFabricacao,
                Eficiencia = vm.Eficiencia,
                SistemaId = vm.SistemaId
            };
            await painelUseCase.UpdateAsync(id, dados, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (PainelSolarNaoEncontradoException)
        {
            return NotFound();
        }
        catch (SistemaNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.SistemaId), "Sistema inválido.");
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await painelUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (PainelSolarNaoEncontradoException)
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
            await painelUseCase.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (PainelSolarNaoEncontradoException)
        {
            return NotFound();
        }
    }

    private async Task FillSistemaSelectAsync(CancellationToken cancellationToken)
    {
        var sistemas = await sistemaUseCase.GetAllAsync(cancellationToken);
        ViewBag.SistemaId = sistemas
            .Select(s => new SelectListItem($"{s.NomeInstalacao} ({s.Cliente.Nome})", s.Id.ToString()))
            .ToList();
    }
}
