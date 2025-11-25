using AutoMapper;
using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;
using Moq;

namespace MBA.EducaOn.GestaoConteudo.Application.Test
{
    public class CursoServiceTests : IDisposable
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICursoRepository> _cursoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CursoService _service;

        public CursoServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _cursoRepositoryMock = new Mock<ICursoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _cursoRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

            _service = new CursoService(_mapperMock.Object, _cursoRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarViewModelQuandoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var conteudo = new ConteudoProgramatico("conteudo", 1, DateTime.Now);
            var cursoDomain = new Curso("Nome", "Descricao", 100m, 10, "Publico", "Objetivo", "Requisitos", conteudo);
            // ensure the domain entity has the id we expect (constructor sets Id via Entity base - if not predictable, compare Nome)
            var vm = new CursoViewModel { Id = cursoDomain.Id, Nome = cursoDomain.Nome };

            _cursoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cursoDomain);
            _mapperMock.Setup(m => m.Map<CursoViewModel>(cursoDomain)).Returns(vm);

            // Act
            var result = await _service.ObterPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vm.Id, result.Id);
            Assert.Equal("Nome", result.Nome);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarListaMapeada()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
            var cursos = new List<Curso>
            {
                new Curso("A","DescricaoA",1,1,"p","o","r", conteudo),
                new Curso("B","DescricaoB",2,2,"p","o","r", conteudo)
            };

            var vms = new List<CursoViewModel>
            {
                new CursoViewModel { Id = cursos[0].Id, Nome = cursos[0].Nome },
                new CursoViewModel { Id = cursos[1].Id, Nome = cursos[1].Nome },
            };

            _cursoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(cursos);
            _mapperMock.Setup(m => m.Map<IEnumerable<CursoViewModel>>(cursos)).Returns(vms);

            // Act
            var result = await _service.ObterTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ExistAsync_Guid_DelegatesToRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ExistAsync(id)).ReturnsAsync(true);

            // Act
            var exists = await _service.ExistAsync(id);

            // Assert
            Assert.True(exists);
            _cursoRepositoryMock.Verify(r => r.ExistAsync(id), Times.Once);
        }

        [Fact]
        public async Task ExistAsync_Nome_DelegatesToRepository()
        {
            // Arrange
            var nome = "curso-x";
            _cursoRepositoryMock.Setup(r => r.ExistAsync(nome)).ReturnsAsync(false);

            // Act
            var exists = await _service.ExistAsync(nome);

            // Assert
            Assert.False(exists);
            _cursoRepositoryMock.Verify(r => r.ExistAsync(nome), Times.Once);
        }

        [Fact]
        public async Task Adicionar_DeveChamarAdicionarECommitERetornarViewModel()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
            var vmInput = new CursoViewModel { Nome = "Novo", Descricao = "d", Valor = 10m, CargaHoraria = 1, PublicoAlvo = "p", Objetivo = "o", Requisitos = "r", ConteudoDescricao = "c", Revisao = 1, DataRevisao = DateTime.Now };
            var domain = new Curso("Novo","d",10m,1,"p","o","r", conteudo);
            var vmResult = new CursoViewModel { Id = domain.Id, Nome = domain.Nome };

            _mapperMock.Setup(m => m.Map<Curso>(It.IsAny<CursoViewModel>())).Returns(domain);
            _mapperMock.Setup(m => m.Map<CursoViewModel>(domain)).Returns(vmResult);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _service.Adicionar(vmInput);

            // Assert
            _cursoRepositoryMock.Verify(r => r.Adicionar(domain), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(vmResult.Id, result.Id);
            Assert.Equal(vmResult.Nome, result.Nome);
        }

        [Fact]
        public async Task Atualizar_DeveChamarAtualizarECommitERetornarViewModel()
        {
            // Arrange
            var vm = new CursoViewModel { Id = Guid.NewGuid(), Nome = "Atualizado", Descricao = "d", Valor = 1m, CargaHoraria = 1, PublicoAlvo = "p", Objetivo = "o", Requisitos = "r", ConteudoDescricao = "c", Revisao = 1, DataRevisao = DateTime.Now };
            var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
            var domain = new Curso("Atualizado","d",1m,1,"p","o","r", conteudo);

            _mapperMock.Setup(m => m.Map<Curso>(vm)).Returns(domain);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _service.Atualizar(vm);

            // Assert
            _cursoRepositoryMock.Verify(r => r.Atualizar(domain), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(vm, result);
        }

        [Fact]
        public async Task Deletar_DeveChamarRepositorioECommit()
        {
            // Arrange
            var id = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.Deletar(id)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            await _service.Deletar(id);

            // Assert
            _cursoRepositoryMock.Verify(r => r.Deletar(id), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task AdicionarAula_DeveAdicionarAulaECommitERetornarCursoViewModel()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
            var curso = new Curso("C","D",1,1,"p","o","r", conteudo);
            var aulaVm = new AulaViewModel { Id = Guid.NewGuid(), CursoId = curso.Id, Codigo = "C1", Titulo = "T1", Descricao = "D1", Ordem = 1 };
            var aulaDomain = new Aula(curso.Id, aulaVm.Codigo, aulaVm.Titulo, aulaVm.Descricao, aulaVm.Ordem);
            var cursoVm = new CursoViewModel { Id = curso.Id, Nome = curso.Nome };

            _cursoRepositoryMock.Setup(r => r.ObterPorIdAsync(aulaVm.CursoId)).ReturnsAsync(curso);
            _mapperMock.Setup(m => m.Map<Aula>(aulaVm)).Returns(aulaDomain);
            _mapperMock.Setup(m => m.Map<CursoViewModel>(curso)).Returns(cursoVm);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _service.AdicionarAula(aulaVm);

            // Assert
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(cursoVm.Id, result.Id);
        }

        [Fact]
        public async Task DeletarAulaAsync_DeveRemoverAulaECommitERetornarCursoViewModel()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
            var curso = new Curso("C","D",1,1,"p","o","r", conteudo);
            var aula = new Aula(curso.Id, "C1", "T1", "D1", 1);
            curso.AdicionarAula(aula);
            var cursoVm = new CursoViewModel { Id = curso.Id, Nome = curso.Nome };

            // The service uses ObterPorIdAsync(id) with the aula id (as implemented),
            // so setup repository to return the curso when asked with aula.Id.
            _cursoRepositoryMock.Setup(r => r.ObterPorIdAsync(aula.Id)).ReturnsAsync(curso);
            _mapperMock.Setup(m => m.Map<CursoViewModel>(curso)).Returns(cursoVm);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            // Act
            var result = await _service.DeletarAulaAsync(aula.Id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
            Assert.Equal(cursoVm.Id, result.Id);
        }

        [Fact]
        public void Dispose_DeveChamarDisposeDoRepositorio()
        {
            // Act
            _service.Dispose();

            // Assert
            _cursoRepositoryMock.Verify(r => r.Dispose(), Times.Once);
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}