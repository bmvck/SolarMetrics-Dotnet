using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarMetrics.Web.UseCase;
using SolarMetrics.Web.ViewModels;

namespace SolarMetrics.Web.Controllers;

[Authorize]
public sealed class VendasController(IClienteUseCase clienteUseCase) : Controller
{
    public async Task<IActionResult> NovosClientes(CancellationToken cancellationToken)
    {
        var clientes = await clienteUseCase.GetAllAsync();
        var clientesViews = clientes.Select(ClienteViewModel.ToResponse).ToList();
        return View(clientesViews);
    }
}
