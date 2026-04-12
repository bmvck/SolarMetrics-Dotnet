using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SolarMetrics.Web.Configuration;

namespace SolarMetrics.Web.Services;

/// <summary>Emite JWT com as mesmas reivindicações do <c>AuthController</c> da API (fallback quando a API não está disponível em Development).</summary>
public sealed class LocalJwtIssuer(IOptions<JwtSettings> jwtOptions)
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public string CreateAccessToken()
    {
        if (string.IsNullOrWhiteSpace(_jwt.Key))
            throw new InvalidOperationException("Jwt:Key não configurada.");

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
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
