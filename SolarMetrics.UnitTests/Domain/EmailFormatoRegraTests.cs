using SolarMetrics.Domain;
using Xunit;

namespace SolarMetrics.UnitTests.Domain;

public sealed class EmailFormatoRegraTests
{
    [Fact]
    public void EhValido_EmailComLocalEDominioComPonto_RetornaTrue()
    {
        var email = "usuario@empresa.com.br";

        var resultado = EmailFormatoRegra.EhValido(email);

        Assert.True(resultado);
    }

    [Fact]
    public void EhValido_StringVazia_RetornaFalse()
    {
        var resultado = EmailFormatoRegra.EhValido(string.Empty);

        Assert.False(resultado);
    }

    [Fact]
    public void EhValido_SemArroba_RetornaFalse()
    {
        var resultado = EmailFormatoRegra.EhValido("usuarioempresa.com");

        Assert.False(resultado);
    }

    [Fact]
    public void EhValido_DominioSemPonto_RetornaFalse()
    {
        var resultado = EmailFormatoRegra.EhValido("user@localhost");

        Assert.False(resultado);
    }
}
