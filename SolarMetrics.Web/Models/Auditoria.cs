namespace SolarMetrics.Web.Models;

public class Auditoria
{
    public long IdAuditoria { get; set; }
    public string NomeTabela { get; set; } = null!;
    public string Operacao { get; set; } = null!;
    public string UsuarioOracle { get; set; } = null!;
    public DateTime DataOperacao { get; set; }
    public string? DadosOld { get; set; }
    public string? DadosNew { get; set; }
}
