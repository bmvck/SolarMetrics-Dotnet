using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SolarMetrics.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarMetrics.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IOptions<JwtSettings> jwtOptions, IHostEnvironment environment) : ControllerBase
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    /// <summary>Emite JWT para desenvolvimento e testes automatizados. Indisponível em Produção.</summary>
    [HttpPost("token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Obter JWT (dev/teste)", Description = "Disponível fora de Produção para Swagger e integração.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token emitido")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Endpoint desabilitado em Produção")]
    public IActionResult PostToken()
    {
        if (environment.IsProduction())
            return NotFound();

        if (string.IsNullOrWhiteSpace(_jwt.Key))
            return Problem("Jwt:Key não configurada.", statusCode: StatusCodes.Status500InternalServerError);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, "api-user"),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N"))
        };
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Math.Max(5, _jwt.ExpirationMinutes)),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = jwt, token_type = "Bearer", expires_in = _jwt.ExpirationMinutes * 60 });
    }
}
