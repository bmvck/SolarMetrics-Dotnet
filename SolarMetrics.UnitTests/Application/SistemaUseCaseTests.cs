using Moq;
using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Repositories;
using SolarMetrics.UseCase;
using Xunit;

namespace SolarMetrics.UnitTests.Application;

public sealed class SistemaUseCaseTests
{
    private readonly Mock<ISistemaRepository> _sistemaRepo = new();
    private readonly Mock<IClienteRepository> _clienteRepo = new();
    private SistemaUseCase CriarSut() => new(_sistemaRepo.Object, _clienteRepo.Object);

    [Fact]
    public async Task CreateAsync_ClienteInexistente_LancaClienteNaoEncontradoException()
    {
        var sut = CriarSut();
        var sistema = new Sistema
        {
            NomeInstalacao = "Inst",
            DataInstalacao = DateTime.UtcNow,
            PotenciaTotal = 100,
            Status = "ATIVO",
            ClienteId = Guid.NewGuid()
        };
        _clienteRepo.Setup(r => r.GetByIdAsync(sistema.ClienteId, It.IsAny<CancellationToken>())).ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<ClienteNaoEncontradoException>(() => sut.CreateAsync(sistema));
    }

    [Fact]
    public async Task GetByIdAsync_NaoExiste_LancaSistemaNaoEncontradoException()
    {
        var sut = CriarSut();
        var id = Guid.NewGuid();
        _sistemaRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Sistema?)null);

        await Assert.ThrowsAsync<SistemaNaoEncontradoException>(() => sut.GetByIdAsync(id));
    }
}
