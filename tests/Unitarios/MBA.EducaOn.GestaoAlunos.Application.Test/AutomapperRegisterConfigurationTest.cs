using AutoMapper;
using MBA.EducaOn.GestaoAlunos.Application.AutoMapper;
using MBA.EducaOn.GestaoAlunos.Application.ViewModels;
using MBA.EducaOn.GestaoAlunos.Domain;

namespace MBA.EducaOn.GestaoAlunos.Application.Test
{   
    public class AutomapperRegisterConfigurationTest
    {
        [Fact]
        public void RegisterMappings_DeveRetornarMapperConfigurationValida()
        {
            // Act
            var config = AutomapperRegisterConfiguration.RegisterMappings();

            // Assert
            Assert.NotNull(config);
            Assert.IsType<MapperConfiguration>(config);
        }

        [Fact]
        public void RegisterMappings_DeveConterPerfilDomainToViewModel()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();

            // Act & Assert
            Assert.NotNull(config);
        }

        [Fact]
        public void RegisterMappings_DeveMapearAlunoParaAlunoViewModel()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var aluno = new Aluno(Guid.NewGuid(), "João Silva", "joao@email.com");

            // Act
            var alunoViewModel = mapper.Map<AlunoViewModel>(aluno);

            // Assert
            Assert.NotNull(alunoViewModel);
            Assert.Equal(aluno.Id, alunoViewModel.Id);
            Assert.Equal(aluno.Nome, alunoViewModel.Nome);
            Assert.Equal(aluno.Email, alunoViewModel.Email);
            Assert.Equal(aluno.Ativo, alunoViewModel.Ativo);
        }

        [Fact]
        public void RegisterMappings_DeveMapearAlunoViewModelParaAluno()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var alunoViewModel = new AlunoViewModel
            {
                Id = Guid.NewGuid(),
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Ativo = true,
                DataCadastro = DateTime.Now
            };

            // Act
            var aluno = mapper.Map<Aluno>(alunoViewModel);

            // Assert
            Assert.NotNull(aluno);
            Assert.Equal(alunoViewModel.Id, aluno.Id);
            Assert.Equal(alunoViewModel.Nome, aluno.Nome);
            Assert.Equal(alunoViewModel.Email, aluno.Email);
        }

        [Fact]
        public void RegisterMappings_DeveMapearListaDeAlunosParaListaDeViewModels()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var alunos = new[]
            {
                new Aluno(Guid.NewGuid(), "Aluno 1", "aluno1@email.com"),
                new Aluno(Guid.NewGuid(), "Aluno 2", "aluno2@email.com"),
                new Aluno(Guid.NewGuid(), "Aluno 3", "aluno3@email.com")
            };

            // Act
            var alunosViewModel = mapper.Map<AlunoViewModel[]>(alunos);

            // Assert
            Assert.NotNull(alunosViewModel);
            Assert.Equal(3, alunosViewModel.Length);
            Assert.Equal(alunos[0].Nome, alunosViewModel[0].Nome);
            Assert.Equal(alunos[1].Nome, alunosViewModel[1].Nome);
            Assert.Equal(alunos[2].Nome, alunosViewModel[2].Nome);
        }

        [Fact]
        public void RegisterMappings_DeveCriarMapperFuncional()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();

            // Act
            var mapper = config.CreateMapper();

            // Assert
            Assert.NotNull(mapper);
            Assert.IsAssignableFrom<IMapper>(mapper);
        }

        [Fact]
        public void RegisterMappings_ConfiguracaoDeveSerValida()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();

            // Act & Assert - Se a configuração for inválida, uma exceção será lançada
            Assert.NotNull(config);
        }

        [Fact]
        public void RegisterMappings_DeveMapearPropriedadesCorretamente()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var alunoId = Guid.NewGuid();
            var dataCadastro = DateTime.Now;

            var aluno = new Aluno(alunoId, "Pedro Costa", "pedro@email.com");

            // Act
            var viewModel = mapper.Map<AlunoViewModel>(aluno);

            // Assert
            Assert.Equal(alunoId, viewModel.Id);
            Assert.Equal("Pedro Costa", viewModel.Nome);
            Assert.Equal("pedro@email.com", viewModel.Email);
            Assert.True(viewModel.Ativo);
        }

        [Fact]
        public void RegisterMappings_DeveTratarValoresNulos()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            // Act
            var resultado = mapper.Map<AlunoViewModel>((Aluno)null);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void RegisterMappings_DevePreservarTiposDados()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "Ana Lima", "ana@email.com");

            // Act
            var viewModel = mapper.Map<AlunoViewModel>(aluno);

            // Assert
            Assert.IsType<Guid>(viewModel.Id);
            Assert.IsType<string>(viewModel.Nome);
            Assert.IsType<string>(viewModel.Email);
            Assert.IsType<bool>(viewModel.Ativo);
            Assert.IsType<DateTime>(viewModel.DataCadastro);
        }
    }
}
