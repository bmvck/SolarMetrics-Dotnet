using Moq;
using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Repositories;
using SolarMetrics.UseCase;
using Xunit;

namespace SolarMetrics.UnitTests.Application;

public sealed class ClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _repo = new();
    private ClienteUseCase CriarSut() => new(_repo.Object);

    private static Cliente NovoCliente(string email = "a@b.com") => new()
    {
        Id = Guid.NewGuid(),
        Nome = "Teste",
        Email = email,
        Telefone = "11999999999",
        TipoUsuario = "ADMIN"
    };

    [Fact]
    public async Task CreateAsync_EmailNovo_PersisteERetornaCliente()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.FindEmailAsync(cliente.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente c, CancellationToken _) => c);

        var resultado = await sut.CreateAsync(cliente);

        Assert.Same(cliente, resultado);
        _repo.Verify(r => r.AddAsync(cliente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EmailInvalido_LancaArgumentException()
    {
        var sut = CriarSut();
        var cliente = NovoCliente("invalido");

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateAsync(cliente));
    }

    [Fact]
    public async Task CreateAsync_EmailDuplicado_LancaEmailDuplicadoException()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.FindEmailAsync(cliente.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(cliente.Email);

        await Assert.ThrowsAsync<EmailDuplicadoException>(() => sut.CreateAsync(cliente));
        _repo.Verify(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetById_Existe_RetornaCliente()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

        var resultado = await sut.GetById(cliente.Id);

        Assert.Same(cliente, resultado);
    }

    [Fact]
    public async Task GetById_NaoExiste_LancaClienteNaoEncontradoException()
    {
        var sut = CriarSut();
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<ClienteNaoEncontradoException>(() => sut.GetById(id));
    }

    [Fact]
    public async Task UpdateAsync_EmailDisponivel_AtualizaCampos()
    {
        var sut = CriarSut();
        var id = Guid.NewGuid();
        var existente = NovoCliente("velho@velho.com");
        existente.Id = id;
        var atualizado = NovoCliente("novo@novo.com");
        atualizado.Id = id;
        atualizado.Nome = "Novo Nome";

        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(existente);
        _repo.Setup(r => r.FindEmailAsync(atualizado.Email, id, It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var resultado = await sut.UpdateAsync(atualizado);

        Assert.Equal("novo@novo.com", resultado.Email);
        Assert.Equal("Novo Nome", resultado.Nome);
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Existe_RemoveViaRepositorio()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _repo.Setup(r => r.DeleteAsync(cliente, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await sut.DeleteAsync(cliente.Id);

        _repo.Verify(r => r.DeleteAsync(cliente, It.IsAny<CancellationToken>()), Times.Once);
    }
}
