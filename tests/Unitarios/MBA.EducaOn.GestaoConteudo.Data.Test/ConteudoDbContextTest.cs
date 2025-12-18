using MBA.EducaOn.GestaoConteudo.Domain;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.GestaoConteudo.Data.Test
{
    public class ConteudoDbContextTest
    {
        private ConteudoDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ConteudoDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return new ConteudoDbContext(options);
        }

        [Fact]
        public void ConteudoDbContext_DeveSerInstanciadoComSucesso()
        {
            // Arrange & Act
            using var context = CreateContext(Guid.NewGuid().ToString());

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Cursos);
            Assert.NotNull(context.Aulas);
            Assert.NotNull(context.Materiais);
        }

        [Fact]
        public async Task Commit_DeveDefinirDataCadastro_QuandoEntidadeForAdicionada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo teste", 1, DateTime.Now);

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("C# Básico", "Curso de C#", 199.90m, 40, "Iniciantes", "Aprender C#", "Nenhum", conteudo);
                context.Cursos.Add(curso);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var curso = await context.Cursos.FirstOrDefaultAsync();
                Assert.NotNull(curso);
                Assert.NotEqual(DateTime.MinValue, curso.DataCadastro);
                Assert.True(curso.DataCadastro <= DateTime.Now);
            }
        }

        [Fact]
        public async Task Commit_NaoDeveAlterarDataCadastro_QuandoEntidadeForModificada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);
            DateTime dataCadastroOriginal;

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("Java", "Curso Java", 299.90m, 50, "Intermediários", "Dominar Java", "Java Básico", conteudo);
                context.Cursos.Add(curso);
                await context.Commit();
                dataCadastroOriginal = curso.DataCadastro;
            }

            await Task.Delay(100);

            using (var context = CreateContext(dbName))
            {
                var curso = await context.Cursos.FirstOrDefaultAsync();
                curso.AlteraStado(false);
                context.Cursos.Update(curso);

                // Act
                await context.Commit();

                // Assert
                Assert.Equal(dataCadastroOriginal, curso.DataCadastro);
            }
        }

        [Fact]
        public async Task Commit_DeveRetornarTrue_QuandoSalvarComSucesso()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);
            var curso = new Curso("Python", "Curso Python", 249.90m, 45, "Todos", "Aprender Python", "Nenhum", conteudo);
            context.Cursos.Add(curso);

            // Act
            var result = await context.Commit();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Commit_DeveRetornarFalse_QuandoNaoHouverMudancas()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            // Act
            var result = await context.Commit();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Cursos_DeveAdicionarERecuperarCurso()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Programação Web", 1, DateTime.Now);
            var cursoId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("JavaScript", "Curso JS", 189.90m, 35, "Iniciantes", "Aprender JS", "Nenhum", conteudo);
                context.Cursos.Add(curso);
                await context.Commit();
                cursoId = curso.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var cursoRecuperado = await context.Cursos.FindAsync(cursoId);

                // Assert
                Assert.NotNull(cursoRecuperado);
                Assert.Equal(cursoId, cursoRecuperado.Id);
                Assert.Equal("JavaScript", cursoRecuperado.Nome);
                Assert.Equal(189.90m, cursoRecuperado.Valor);
            }
        }

        [Fact]
        public async Task Aulas_DeveAdicionarERecuperarAula()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();
            var aulaId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var aula = new Aula(cursoId, "AULA01", "Introdução", "Primeira aula", 1);
                context.Aulas.Add(aula);
                await context.Commit();
                aulaId = aula.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var aulaRecuperada = await context.Aulas.FindAsync(aulaId);

                // Assert
                Assert.NotNull(aulaRecuperada);
                Assert.Equal(aulaId, aulaRecuperada.Id);
                Assert.Equal(cursoId, aulaRecuperada.CursoId);
                Assert.Equal("AULA01", aulaRecuperada.Codigo);
                Assert.Equal("Introdução", aulaRecuperada.Titulo);
            }
        }

        [Fact]
        public async Task Materiais_DeveAdicionarERecuperarMaterial()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var aulaId = Guid.NewGuid();
            var materialId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var cursoId = Guid.NewGuid();
                var aula = new Aula(cursoId, "AULA02", "Variáveis", "Segunda aula", 2);
                context.Aulas.Add(aula);
                await context.Commit();

                var material = new Material("Slides", "Material de apoio", "http://exemplo.com/slides.pdf", "http://exemplo.com", aula);
                context.Materiais.Add(material);
                await context.Commit();
                materialId = material.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var materialRecuperado = await context.Materiais.FindAsync(materialId);

                // Assert
                Assert.NotNull(materialRecuperado);
                Assert.Equal("Slides", materialRecuperado.Nome);
                Assert.Equal("Material de apoio", materialRecuperado.Descricao);
            }
        }

        [Fact]
        public async Task Commit_DevePersistirMultiplasEntidades()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);

            using (var context = CreateContext(dbName))
            {
                var curso1 = new Curso("Curso 1", "Desc 1", 100m, 10, "P1", "O1", "R1", conteudo);
                var curso2 = new Curso("Curso 2", "Desc 2", 200m, 20, "P2", "O2", "R2", conteudo);
                var curso3 = new Curso("Curso 3", "Desc 3", 300m, 30, "P3", "O3", "R3", conteudo);

                context.Cursos.AddRange(curso1, curso2, curso3);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var cursos = await context.Cursos.ToListAsync();
                Assert.Equal(3, cursos.Count);
            }
        }

        [Fact]
        public async Task Commit_DeveManterIntegridadeEntreCursoEAulas()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("Node.js", "Curso Node", 299.90m, 40, "Devs", "Aprender Node", "JavaScript", conteudo);
                var aula1 = new Aula(curso.Id, "N01", "Intro", "Introdução", 1);
                var aula2 = new Aula(curso.Id, "N02", "Express", "Framework Express", 2);

                curso.AdicionarAula(aula1);
                curso.AdicionarAula(aula2);

                context.Cursos.Add(curso);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var cursoRecuperado = await context.Cursos
                    .Include(c => c.Aulas)
                    .FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(cursoRecuperado);
                Assert.Equal(2, cursoRecuperado.Aulas.Count);
                Assert.Contains(cursoRecuperado.Aulas, a => a.Codigo == "N01");
                Assert.Contains(cursoRecuperado.Aulas, a => a.Codigo == "N02");
            }
        }

        [Fact]
        public async Task Cursos_DeveAtualizarStatusDoCurso()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);
            var cursoId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("TypeScript", "Curso TS", 249.90m, 35, "Devs", "Aprender TS", "JavaScript", conteudo);
                context.Cursos.Add(curso);
                await context.Commit();
                cursoId = curso.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var curso = await context.Cursos.FindAsync(cursoId);
                curso.AlteraStado(false);
                context.Cursos.Update(curso);
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var cursoAtualizado = await context.Cursos.FindAsync(cursoId);
                Assert.False(cursoAtualizado.Ativo);
            }
        }

        [Fact]
        public async Task Aulas_DeveAtualizarStatusDaAula()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();
            var aulaId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var aula = new Aula(cursoId, "AULA03", "Loops", "Estruturas de repetição", 3);
                context.Aulas.Add(aula);
                await context.Commit();
                aulaId = aula.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var aula = await context.Aulas.FindAsync(aulaId);
                aula.AlteraStado(false);
                context.Aulas.Update(aula);
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var aulaAtualizada = await context.Aulas.FindAsync(aulaId);
                Assert.False(aulaAtualizada.Ativo);
            }
        }

        [Fact]
        public async Task Commit_DeveDefinirDataCadastroParaTodasAsEntidades()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var conteudo = new ConteudoProgramatico("Conteúdo", 1, DateTime.Now);

            using (var context = CreateContext(dbName))
            {
                var curso = new Curso("Angular", "Curso Angular", 349.90m, 50, "Devs", "Aprender Angular", "TypeScript", conteudo);
                var aula = new Aula(curso.Id, "ANG01", "Componentes", "Aula sobre componentes", 1);

                context.Cursos.Add(curso);
                context.Aulas.Add(aula);

                // Act
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var curso = await context.Cursos.FirstOrDefaultAsync();
                var aula = await context.Aulas.FirstOrDefaultAsync();

                Assert.NotEqual(DateTime.MinValue, curso.DataCadastro);
                Assert.NotEqual(DateTime.MinValue, aula.DataCadastro);
            }
        }

        [Fact]
        public async Task Commit_DeveManterIntegridadeEntreAulaEMateriais()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var cursoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aula = new Aula(cursoId, "AULA04", "Arrays", "Trabalhando com arrays", 4);
                var material1 = new Material("PDF", "Apostila", "http://exemplo.com/apostila.pdf", "", aula);
                var material2 = new Material("Vídeo", "Videoaula", "http://exemplo.com/video.mp4", "", aula);

                aula.AdicionarMaterial(material1);
                aula.AdicionarMaterial(material2);

                context.Aulas.Add(aula);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var aulaRecuperada = await context.Aulas
                    .Include(a => a.Materiais)
                    .FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(aulaRecuperada);
                Assert.Equal(2, aulaRecuperada.Materiais.Count);
                Assert.Contains(aulaRecuperada.Materiais, m => m.Nome == "PDF");
                Assert.Contains(aulaRecuperada.Materiais, m => m.Nome == "Vídeo");
            }
        }
    }
}
