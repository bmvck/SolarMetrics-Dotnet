using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SolarMetrics.Web.Auth;
using SolarMetrics.Web.Configuration;
using SolarMetrics.Web.Services;

namespace SolarMetrics.Web.Controllers;

[AllowAnonymous]
public sealed class AccountController(
    AdminTokenAcquisitionService tokenAcquisition,
    IOptions<JwtSettings> jwtOptions,
    IHostEnvironment environment) : Controller
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl)
            ? Url.Action("Index", "Dashboard", new { area = "Admin" })!
            : returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? returnUrl, CancellationToken cancellationToken)
    {
        var acquisition = await tokenAcquisition.GetAccessTokenAsync(cancellationToken);
        if (string.IsNullOrEmpty(acquisition.Token))
        {
            ModelState.AddModelError(
                string.Empty,
                acquisition.FailureMessage
                ?? "Não foi possível obter o token. Em produção configure Api:BaseUrl e um endpoint de emissão de JWT válido.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var token = acquisition.Token;

        var minutes = Math.Max(5, _jwt.ExpirationMinutes);
        Response.Cookies.Append(AdminAuthCookie.Name, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = environment.IsProduction(),
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            MaxAge = TimeSpan.FromMinutes(minutes),
            Path = "/"
        });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AdminAuthCookie.Name, new CookieOptions { Path = "/" });
        return RedirectToAction("Index", "Home");
    }
}
