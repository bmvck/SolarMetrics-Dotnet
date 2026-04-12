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
public sealed class MonitoramentoController(
    ISensorRepository sensorRepository,
    IMonitoramentoUseCase monitoramentoUseCase) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await monitoramentoUseCase.GetAllAsync(cancellationToken));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await monitoramentoUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (MonitoramentoNaoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await FillSensorSelectAsync(cancellationToken);
        return View(new MonitoramentoFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MonitoramentoFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSensorSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var entity = new Monitoramento
            {
                Periodo = vm.Periodo,
                ValorLido = vm.ValorLido,
                MediaLeitura = vm.MediaLeitura,
                MaximaLeitura = vm.MaximaLeitura,
                SensorId = vm.SensorId
            };
            await monitoramentoUseCase.CreateAsync(entity, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (SensorNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.SensorId), "Sensor inválido.");
            await FillSensorSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var m = await monitoramentoUseCase.GetByIdAsync(id, cancellationToken);
            await FillSensorSelectAsync(cancellationToken);
            return View(new MonitoramentoFormViewModel
            {
                Id = m.Id,
                Periodo = m.Periodo,
                ValorLido = m.ValorLido,
                MediaLeitura = m.MediaLeitura,
                MaximaLeitura = m.MaximaLeitura,
                SensorId = m.SensorId
            });
        }
        catch (MonitoramentoNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, MonitoramentoFormViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillSensorSelectAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var dados = new Monitoramento
            {
                Periodo = vm.Periodo,
                ValorLido = vm.ValorLido,
                MediaLeitura = vm.MediaLeitura,
                MaximaLeitura = vm.MaximaLeitura,
                SensorId = vm.SensorId
            };
            await monitoramentoUseCase.UpdateAsync(id, dados, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (MonitoramentoNaoEncontradoException)
        {
            return NotFound();
        }
        catch (SensorNaoEncontradoException)
        {
            ModelState.AddModelError(nameof(vm.SensorId), "Sensor inválido.");
            await FillSensorSelectAsync(cancellationToken);
            return View(vm);
        }
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await monitoramentoUseCase.GetByIdAsync(id, cancellationToken));
        }
        catch (MonitoramentoNaoEncontradoException)
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
            await monitoramentoUseCase.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (MonitoramentoNaoEncontradoException)
        {
            return NotFound();
        }
    }

    private async Task FillSensorSelectAsync(CancellationToken cancellationToken)
    {
        var sensores = await sensorRepository.GetAllAsync(cancellationToken);
        ViewBag.SensorId = sensores
            .Select(s => new SelectListItem($"{s.Tipo} — {s.Sistema.NomeInstalacao}", s.Id.ToString()))
            .ToList();
    }
}
