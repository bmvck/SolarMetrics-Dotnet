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
public sealed class SensorController(ISistemaUseCase sistemaUseCase, ISensorUseCase sensorUseCase) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await sensorUseCase.GetAllAsync(cancellationToken));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await sensorUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (SensorNaoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await FillSistemaSelectAsync(cancellationToken);
        return View(new SensorFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SensorFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var entity = new Sensor
            {
                Tipo = vm.Tipo,
                Status = vm.Status,
                Localizacao = vm.Localizacao,
                SistemaId = vm.SistemaId
            };
            await sensorUseCase.CreateAsync(entity, cancellationToken);
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
            var sensor = await sensorUseCase.GetByIdAsync(id, cancellationToken);
            await FillSistemaSelectAsync(cancellationToken);
            return View(new SensorFormViewModel
            {
                Id = sensor.Id,
                Tipo = sensor.Tipo,
                Status = sensor.Status,
                Localizacao = sensor.Localizacao,
                SistemaId = sensor.SistemaId
            });
        }
        catch (SensorNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SensorFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSistemaSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var dados = new Sensor
            {
                Tipo = vm.Tipo,
                Status = vm.Status,
                Localizacao = vm.Localizacao,
                SistemaId = vm.SistemaId
            };
            await sensorUseCase.UpdateAsync(id, dados, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SensorNaoEncontradoException)
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
            return View(await sensorUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (SensorNaoEncontradoException)
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
            await sensorUseCase.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SensorNaoEncontradoException)
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
