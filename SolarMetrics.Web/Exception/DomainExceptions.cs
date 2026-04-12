namespace SolarMetrics.Exceptions;

public sealed class SistemaNaoEncontradoException : Exception
{
    public SistemaNaoEncontradoException() : base("Sistema não encontrado") { }
}

public sealed class PainelSolarNaoEncontradoException : Exception
{
    public PainelSolarNaoEncontradoException() : base("Painel solar não encontrado") { }
}

public sealed class SensorNaoEncontradoException : Exception
{
    public SensorNaoEncontradoException() : base("Sensor não encontrado") { }
}

public sealed class MonitoramentoNaoEncontradoException : Exception
{
    public MonitoramentoNaoEncontradoException() : base("Monitoramento não encontrado") { }
}
