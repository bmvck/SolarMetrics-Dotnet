using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Web;
using SolarMetrics.Web.Areas.Admin.ViewModels;

namespace SolarMetrics.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public sealed class DashboardController(SolarMetricsContext db, ILogger<DashboardController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var vm = new DashboardViewModel
            {
                TotalClientes = await db.Clientes.CountAsync(cancellationToken),
                TotalSistemas = await db.Sistemas.CountAsync(cancellationToken),
                TotalPaineis = await db.PaineisSolares.CountAsync(cancellationToken),
                TotalSensores = await db.Sensores.CountAsync(cancellationToken),
                TotalMonitoramentos = await db.Monitoramentos.CountAsync(cancellationToken)
            };
            return View(vm);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao carregar contagens do dashboard (Oracle/wallet).");
            ViewData["DbError"] =
                "Não foi possível conectar ao Oracle. Verifique ConnectionStrings__OracleDb (senha real, não placeholder), " +
                "a pasta /home/site/wwwroot/wallet no App Service e reinicie o Web App.";
            return View("DatabaseError");
        }
    }
}
