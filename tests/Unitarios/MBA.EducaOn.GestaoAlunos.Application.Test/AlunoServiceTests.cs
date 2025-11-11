using AutoMapper;
using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoAlunos.Application.Services;
using MBA.EducaOn.GestaoAlunos.Application.ViewModels;
using MBA.EducaOn.GestaoAlunos.Domain;
using MBA.EducaOn.GestaoAlunos.Domain.Interfaces.Repositories;
using Moq;

namespace MBA.EducaOn.GestaoAlunos.Application.Test;

public class AlunoServiceTests : IDisposable
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IAlunoService _alunoService;

    public AlunoServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _alunoRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _alunoService = new AlunoService(_mapperMock.Object, _alunoRepositoryMock.Object);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarAlunoViewModel()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var aluno = new Aluno(alunoId, "Teste", "teste@email.com");
        var alunoViewModel = new AlunoViewModel { Id = alunoId, Nome = "Teste", Email = "teste@email.com" };

        _alunoRepositoryMock.Setup(r => r.ObterPorIdAsync(alunoId)).ReturnsAsync(aluno);
        _mapperMock.Setup(m => m.Map<AlunoViewModel>(aluno)).Returns(alunoViewModel);

        // Act
        var result = await _alunoService.ObterPorId(alunoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alunoId, result.Id);
        Assert.Equal("Teste", result.Nome);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarListaDeAlunoViewModel()
    {
        // Arrange
        var alunos = new List<Aluno>
        {
            new Aluno(Guid.NewGuid(), "Aluno1", "a1@email.com"),
            new Aluno(Guid.NewGuid(), "Aluno2", "a2@email.com")
        };
        var alunosViewModel = alunos.Select(a => new AlunoViewModel { Id = a.Id, Nome = a.Nome, Email = a.Email }).ToList();

        _alunoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(alunos);
        _mapperMock.Setup(m => m.Map<IEnumerable<AlunoViewModel>>(alunos)).Returns(alunosViewModel);

        // Act
        var result = await _alunoService.ObterTodos();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Adicionar_DeveChamarAdicionarECommit()
    {
        // Arrange
        var alunoViewModel = new AlunoViewModel { Id = Guid.NewGuid(), Nome = "Novo", Email = "novo@email.com" };
        var aluno = new Aluno(alunoViewModel.Id, alunoViewModel.Nome, alunoViewModel.Email);

        _mapperMock.Setup(m => m.Map<Aluno>(alunoViewModel)).Returns(aluno);
        _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

        // Act
        await _alunoService.Adicionar(alunoViewModel);

        // Assert
        _alunoRepositoryMock.Verify(r => r.Adicionar(aluno), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task Atualizar_DeveChamarAtualizarECommitERetornarViewModel()
    {
        // Arrange
        var alunoViewModel = new AlunoViewModel { Id = Guid.NewGuid(), Nome = "Atualizado", Email = "atualizado@email.com" };
        var aluno = new Aluno(alunoViewModel.Id, alunoViewModel.Nome, alunoViewModel.Email);

        _mapperMock.Setup(m => m.Map<Aluno>(alunoViewModel)).Returns(aluno);
        _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

        // Act
        var result = await _alunoService.Atualizar(alunoViewModel);

        // Assert
        _alunoRepositoryMock.Verify(r => r.Atualizar(aluno), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(alunoViewModel, result);
    }

    [Fact]
    public void Dispose_DeveChamarDisposeDoRepositorio()
    {
        // Act
        _alunoService.Dispose();

        // Assert
        _alunoRepositoryMock.Verify(r => r.Dispose(), Times.Once);
    }

    public void Dispose()
    {
        _alunoService.Dispose();
    }
}