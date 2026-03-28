namespace SolarMetrics.Domain;

/// <summary>Regra de domínio pura para validação de formato de e-mail (sem dependências de infraestrutura).</summary>
public static class EmailFormatoRegra
{
    public static bool EhValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmed = email.Trim();
        var at = trimmed.IndexOf('@');
        if (at <= 0 || at == trimmed.Length - 1)
            return false;

        var local = trimmed[..at];
        var domain = trimmed[(at + 1)..];
        if (local.Length == 0 || domain.Length < 3)
            return false;

        if (!domain.Contains('.'))
            return false;

        return true;
    }
}
