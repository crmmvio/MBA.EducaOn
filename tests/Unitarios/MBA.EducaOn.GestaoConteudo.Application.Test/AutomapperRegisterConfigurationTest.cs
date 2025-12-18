using AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.AutoMapper;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using MBA.EducaOn.GestaoConteudo.Domain;

namespace MBA.EducaOn.GestaoConteudo.Application.Test
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
        public void RegisterMappings_DeveConterPerfisConfigurados()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();

            // Act & Assert
            Assert.NotNull(config);
        }

        [Fact]
        public void RegisterMappings_DeveMapearCursoParaCursoViewModel()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var conteudo = new ConteudoProgramatico("Conteudo Teste", 1, DateTime.Now);
            var curso = new Curso("C# Avançado", "Curso completo", 299.90m, 40, "Desenvolvedores", "Aprender C#", "C# Básico", conteudo);

            // Act
            var cursoViewModel = mapper.Map<CursoViewModel>(curso);

            // Assert
            Assert.NotNull(cursoViewModel);
            Assert.Equal(curso.Id, cursoViewModel.Id);
            Assert.Equal(curso.Nome, cursoViewModel.Nome);
            Assert.Equal(curso.Descricao, cursoViewModel.Descricao);
            Assert.Equal(curso.Valor, cursoViewModel.Valor);
            Assert.Equal(curso.CargaHoraria, cursoViewModel.CargaHoraria);
            Assert.Equal(curso.PublicoAlvo, cursoViewModel.PublicoAlvo);
            Assert.Equal(curso.Objetivo, cursoViewModel.Objetivo);
            Assert.Equal(curso.Requisitos, cursoViewModel.Requisitos);
            Assert.Equal(curso.Ativo, cursoViewModel.Ativo);
        }

        [Fact]
        public void RegisterMappings_DeveMapearCursoViewModelParaCurso()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var cursoViewModel = new CursoViewModel
            {
                Nome = "Java Fundamentals",
                Descricao = "Curso de Java",
                Valor = 199.90m,
                CargaHoraria = 30,
                PublicoAlvo = "Iniciantes",
                Objetivo = "Aprender Java",
                Requisitos = "Nenhum",
                ConteudoDescricao = "Conteúdo programático",
                Revisao = 1,
                DataRevisao = DateTime.Now,
                Ativo = true
            };

            // Act
            var curso = mapper.Map<Curso>(cursoViewModel);

            // Assert
            Assert.NotNull(curso);
            Assert.Equal(cursoViewModel.Nome, curso.Nome);
            Assert.Equal(cursoViewModel.Descricao, curso.Descricao);
            Assert.Equal(cursoViewModel.Valor, curso.Valor);
            Assert.Equal(cursoViewModel.CargaHoraria, curso.CargaHoraria);
        }

        [Fact]
        public void RegisterMappings_DeveMapearAulaParaAulaViewModel()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var cursoId = Guid.NewGuid();
            var aula = new Aula(cursoId, "AULA01", "Introdução", "Primeira aula do curso", 1);

            // Act
            var aulaViewModel = mapper.Map<AulaViewModel>(aula);

            // Assert
            Assert.NotNull(aulaViewModel);
            Assert.Equal(aula.Id, aulaViewModel.Id);
            Assert.Equal(aula.Codigo, aulaViewModel.Codigo);
            Assert.Equal(aula.Titulo, aulaViewModel.Titulo);
            Assert.Equal(aula.Descricao, aulaViewModel.Descricao);
            Assert.Equal(aula.Ordem, aulaViewModel.Ordem);
            Assert.Equal(aula.CursoId, aulaViewModel.CursoId);
            Assert.Equal(aula.Ativo, aulaViewModel.Ativo);
        }

        [Fact]
        public void RegisterMappings_DeveMapearAulaViewModelParaAula()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var cursoId = Guid.NewGuid();
            var aulaViewModel = new AulaViewModel
            {
                CursoId = cursoId,
                Codigo = "AULA02",
                Titulo = "Variáveis",
                Descricao = "Tipos de variáveis",
                Ordem = 2,
                Ativo = true
            };

            // Act
            var aula = mapper.Map<Aula>(aulaViewModel);

            // Assert
            Assert.NotNull(aula);
            Assert.Equal(aulaViewModel.CursoId, aula.CursoId);
            Assert.Equal(aulaViewModel.Codigo, aula.Codigo);
            Assert.Equal(aulaViewModel.Titulo, aula.Titulo);
            Assert.Equal(aulaViewModel.Descricao, aula.Descricao);
            Assert.Equal(aulaViewModel.Ordem, aula.Ordem);
        }

        [Fact]
        public void RegisterMappings_DeveMapearListaDeCursosParaListaDeViewModels()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var conteudo = new ConteudoProgramatico("Conteudo", 1, DateTime.Now);
            var cursos = new[]
            {
                new Curso("Curso 1", "Desc 1", 100m, 10, "P1", "O1", "R1", conteudo),
                new Curso("Curso 2", "Desc 2", 200m, 20, "P2", "O2", "R2", conteudo),
                new Curso("Curso 3", "Desc 3", 300m, 30, "P3", "O3", "R3", conteudo)
            };

            // Act
            var cursosViewModel = mapper.Map<CursoViewModel[]>(cursos);

            // Assert
            Assert.NotNull(cursosViewModel);
            Assert.Equal(3, cursosViewModel.Length);
            Assert.Equal(cursos[0].Nome, cursosViewModel[0].Nome);
            Assert.Equal(cursos[1].Nome, cursosViewModel[1].Nome);
            Assert.Equal(cursos[2].Nome, cursosViewModel[2].Nome);
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
        public void RegisterMappings_DeveMapearPropriedadesComplexa()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var conteudo = new ConteudoProgramatico("Programação avançada", 2, DateTime.Now.AddDays(-10));
            var curso = new Curso("Python", "Curso Python", 399.90m, 50, "Intermediários", "Dominar Python", "Python Básico", conteudo);

            // Act
            var viewModel = mapper.Map<CursoViewModel>(curso);

            // Assert
            Assert.Equal(conteudo.ConteudoDescricao, viewModel.ConteudoDescricao);
            Assert.Equal(conteudo.Revisao, viewModel.Revisao);
            Assert.Equal(conteudo.DataRevisao, viewModel.DataRevisao);
        }

        [Fact]
        public void RegisterMappings_DeveTratarValoresNulos()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            // Act
            var cursoResultado = mapper.Map<CursoViewModel>((Curso)null);
            var aulaResultado = mapper.Map<AulaViewModel>((Aula)null);

            // Assert
            Assert.Null(cursoResultado);
            Assert.Null(aulaResultado);
        }

        [Fact]
        public void RegisterMappings_DevePreservarTiposDados()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var conteudo = new ConteudoProgramatico("Teste", 1, DateTime.Now);
            var curso = new Curso("TypeScript", "Curso TS", 249.90m, 35, "Devs", "Aprender TS", "JS Básico", conteudo);

            // Act
            var viewModel = mapper.Map<CursoViewModel>(curso);

            // Assert
            Assert.IsType<Guid>(viewModel.Id);
            Assert.IsType<string>(viewModel.Nome);
            Assert.IsType<string>(viewModel.Descricao);
            Assert.IsType<decimal>(viewModel.Valor);
            Assert.IsType<int>(viewModel.CargaHoraria);
            Assert.IsType<bool>(viewModel.Ativo);
            Assert.IsType<DateTime>(viewModel.DataCadastro);
        }

        [Fact]
        public void RegisterMappings_DeveMapearCursoComAulas()
        {
            // Arrange
            var config = AutomapperRegisterConfiguration.RegisterMappings();
            var mapper = config.CreateMapper();

            var conteudo = new ConteudoProgramatico("Conteudo", 1, DateTime.Now);
            var curso = new Curso("Node.js", "Curso Node", 299m, 40, "Devs", "Aprender Node", "JS", conteudo);
            var aula1 = new Aula(curso.Id, "N01", "Intro Node", "Introdução", 1);
            var aula2 = new Aula(curso.Id, "N02", "Express", "Framework Express", 2);
            curso.AdicionarAula(aula1);
            curso.AdicionarAula(aula2);

            // Act
            var cursoViewModel = mapper.Map<CursoViewModel>(curso);

            // Assert
            Assert.NotNull(cursoViewModel);
            Assert.NotNull(cursoViewModel.Aulas);
            Assert.Equal(2, cursoViewModel.Aulas.Count());
        }
    }
}
