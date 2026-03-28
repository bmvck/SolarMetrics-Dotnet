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
        _repo.Setup(r => r.FindEmailAsync(cliente.Email, null)).ReturnsAsync((string?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Cliente>())).ReturnsAsync((Cliente c) => c);

        var resultado = await sut.CreateAsync(cliente);

        Assert.Same(cliente, resultado);
        _repo.Verify(r => r.AddAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EmailDuplicado_LancaEmailDuplicadoException()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.FindEmailAsync(cliente.Email, null)).ReturnsAsync(cliente.Email);

        var ex = await Assert.ThrowsAsync<EmailDuplicadoException>(() => sut.CreateAsync(cliente));

        Assert.NotNull(ex);
        _repo.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task GetById_Existe_RetornaCliente()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);

        var resultado = await sut.GetById(cliente.Id);

        Assert.Same(cliente, resultado);
    }

    [Fact]
    public async Task GetById_NaoExiste_LancaClienteNaoEncontradoException()
    {
        var sut = CriarSut();
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Cliente?)null);

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

        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existente);
        _repo.Setup(r => r.FindEmailAsync(atualizado.Email, id)).ReturnsAsync((string?)null);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var resultado = await sut.UpdateAsync(atualizado);

        Assert.Equal("novo@novo.com", resultado.Email);
        Assert.Equal("Novo Nome", resultado.Nome);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_EmailDeOutroCliente_LancaEmailDuplicadoException()
    {
        var sut = CriarSut();
        var id = Guid.NewGuid();
        var existente = NovoCliente();
        existente.Id = id;
        var payload = NovoCliente("outro@email.com");
        payload.Id = id;

        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existente);
        _repo.Setup(r => r.FindEmailAsync(payload.Email, id)).ReturnsAsync(payload.Email);

        await Assert.ThrowsAsync<EmailDuplicadoException>(() => sut.UpdateAsync(payload));
    }

    [Fact]
    public async Task DeleteAsync_Existe_RemoveViaRepositorio()
    {
        var sut = CriarSut();
        var cliente = NovoCliente();
        _repo.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
        _repo.Setup(r => r.DeleteAsync(cliente)).Returns(Task.CompletedTask);

        await sut.DeleteAsync(cliente.Id);

        _repo.Verify(r => r.DeleteAsync(cliente), Times.Once);
    }
}
