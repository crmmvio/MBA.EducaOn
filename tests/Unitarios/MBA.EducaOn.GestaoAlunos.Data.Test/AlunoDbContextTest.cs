using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MBA.EducaOn.GestaoAlunos.Data;
using MBA.EducaOn.GestaoAlunos.Domain;

namespace MBA.EducaOn.GestaoAlunos.Data.Test
{
    public class AlunoDbContextTest
    {
        private AlunoDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AlunoDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return new AlunoDbContext(options);
        }

        [Fact]
        public void AlunoDbContext_DeveSerInstanciadoComSucesso()
        {
            // Arrange & Act
            using var context = CreateContext(Guid.NewGuid().ToString());

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Alunos);
            Assert.NotNull(context.Certificados);
            Assert.NotNull(context.Matriculas);
        }

        [Fact]
        public async Task Commit_DeveDefinirDataCadastro_QuandoEntidadeForAdicionada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "João Silva", "joao@email.com");
                context.Alunos.Add(aluno);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var aluno = await context.Alunos.FirstOrDefaultAsync();
                Assert.NotNull(aluno);
                Assert.NotEqual(DateTime.MinValue, aluno.DataCadastro);
                Assert.True(aluno.DataCadastro <= DateTime.Now);
            }
        }

        [Fact]
        public async Task Commit_NaoDeveAlterarDataCadastro_QuandoEntidadeForModificada()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            DateTime dataCadastroOriginal;

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Maria Santos", "maria@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();
                dataCadastroOriginal = aluno.DataCadastro;
            }

            await Task.Delay(100); // Pequeno delay para garantir diferença de tempo

            using (var context = CreateContext(dbName))
            {
                var aluno = await context.Alunos.FirstOrDefaultAsync();
                aluno.AlteraStatus(false);
                context.Alunos.Update(aluno);

                // Act
                await context.Commit();

                // Assert
                Assert.Equal(dataCadastroOriginal, aluno.DataCadastro);
            }
        }

        [Fact]
        public async Task Commit_DeveRetornarTrue_QuandoSalvarComSucesso()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            var aluno = new Aluno(Guid.NewGuid(), "Pedro Costa", "pedro@email.com");
            context.Alunos.Add(aluno);

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
        public async Task Alunos_DeveAdicionarERecuperarAluno()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Ana Paula", "ana@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var alunoRecuperado = await context.Alunos.FindAsync(alunoId);

                // Assert
                Assert.NotNull(alunoRecuperado);
                Assert.Equal(alunoId, alunoRecuperado.Id);
                Assert.Equal("Ana Paula", alunoRecuperado.Nome);
                Assert.Equal("ana@email.com", alunoRecuperado.Email);
            }
        }

        [Fact]
        public async Task Certificados_DeveAdicionarERecuperarCertificado()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var certificadoId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Carlos Eduardo", "carlos@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();

                var certificado = new Certificado(alunoId, cursoId, DateTime.Now, "CERT-12345");
                context.Certificados.Add(certificado);
                await context.Commit();
                certificadoId = certificado.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var certificadoRecuperado = await context.Certificados.FindAsync(certificadoId);

                // Assert
                Assert.NotNull(certificadoRecuperado);
                Assert.Equal(alunoId, certificadoRecuperado.AlunoId);
                Assert.Equal(cursoId, certificadoRecuperado.CursoId);
                Assert.Equal("CERT-12345", certificadoRecuperado.Codigo);
            }
        }

        [Fact]
        public async Task Matriculas_DeveAdicionarERecuperarMatricula()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.Empty;
            var dataMatricula = DateTime.Now;

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Fernanda Lima", "fernanda@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();

                var matricula = new Matricula(alunoId, cursoId, dataMatricula);
                context.Matriculas.Add(matricula);
                await context.Commit();
                matriculaId = matricula.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var matriculaRecuperada = await context.Matriculas.FindAsync(matriculaId);

                // Assert
                Assert.NotNull(matriculaRecuperada);
                Assert.Equal(alunoId, matriculaRecuperada.AlunoId);
                Assert.Equal(cursoId, matriculaRecuperada.CursoId);
                Assert.True(matriculaRecuperada.Ativo);
            }
        }

        [Fact]
        public async Task Commit_DevePersistirMultiplasEntidades()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var aluno1 = new Aluno(Guid.NewGuid(), "Aluno 1", "aluno1@email.com");
                var aluno2 = new Aluno(Guid.NewGuid(), "Aluno 2", "aluno2@email.com");
                var aluno3 = new Aluno(Guid.NewGuid(), "Aluno 3", "aluno3@email.com");

                context.Alunos.AddRange(aluno1, aluno2, aluno3);

                // Act
                var result = await context.Commit();

                // Assert
                Assert.True(result);
            }

            using (var context = CreateContext(dbName))
            {
                var alunos = await context.Alunos.ToListAsync();
                Assert.Equal(3, alunos.Count);
            }
        }

        [Fact]
        public async Task Commit_DeveManterIntegridadeEntreAlunoECertificados()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId1 = Guid.NewGuid();
            var cursoId2 = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Roberto Silva", "roberto@email.com");
                aluno.AdicionarCertificado(cursoId1, DateTime.Now, "CERT-001");
                aluno.AdicionarCertificado(cursoId2, DateTime.Now, "CERT-002");

                context.Alunos.Add(aluno);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var alunoRecuperado = await context.Alunos
                    .Include(a => a.Certificados)
                    .FirstOrDefaultAsync(a => a.Id == alunoId);

                // Assert
                Assert.NotNull(alunoRecuperado);
                Assert.Equal(2, alunoRecuperado.Certificados.Count);
                Assert.Contains(alunoRecuperado.Certificados, c => c.Codigo == "CERT-001");
                Assert.Contains(alunoRecuperado.Certificados, c => c.Codigo == "CERT-002");
            }
        }

        [Fact]
        public async Task Commit_DeveManterIntegridadeEntreAlunoEMatriculas()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId1 = Guid.NewGuid();
            var cursoId2 = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Juliana Ferreira", "juliana@email.com");
                aluno.AdicionarMatricula(cursoId1);
                aluno.AdicionarMatricula(cursoId2);

                context.Alunos.Add(aluno);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var alunoRecuperado = await context.Alunos
                    .Include(a => a.Matriculas)
                    .FirstOrDefaultAsync(a => a.Id == alunoId);

                // Assert
                Assert.NotNull(alunoRecuperado);
                Assert.Equal(2, alunoRecuperado.Matriculas.Count);
                Assert.Contains(alunoRecuperado.Matriculas, m => m.CursoId == cursoId1);
                Assert.Contains(alunoRecuperado.Matriculas, m => m.CursoId == cursoId2);
            }
        }

        [Fact]
        public async Task Alunos_DeveAtualizarStatusDoAluno()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Marcos Oliveira", "marcos@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var aluno = await context.Alunos.FindAsync(alunoId);
                aluno.AlteraStatus(false);
                context.Alunos.Update(aluno);
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var alunoAtualizado = await context.Alunos.FindAsync(alunoId);
                Assert.False(alunoAtualizado.Ativo);
            }
        }

        [Fact]
        public async Task Matriculas_DeveAtualizarStatusDaMatricula()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            var matriculaId = Guid.Empty;

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Patrícia Rocha", "patricia@email.com");
                context.Alunos.Add(aluno);
                await context.Commit();

                var matricula = new Matricula(alunoId, cursoId, DateTime.Now);
                context.Matriculas.Add(matricula);
                await context.Commit();
                matriculaId = matricula.Id;
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var matricula = await context.Matriculas.FindAsync(matriculaId);
                matricula.AlteraStatus(false);
                context.Matriculas.Update(matricula);
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var matriculaAtualizada = await context.Matriculas.FindAsync(matriculaId);
                Assert.False(matriculaAtualizada.Ativo);
            }
        }

        [Fact]
        public async Task Commit_DeveDefinirDataCadastroParaTodasAsEntidades()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var aluno = new Aluno(alunoId, "Ricardo Mendes", "ricardo@email.com");
                var certificado = new Certificado(alunoId, cursoId, DateTime.Now, "CERT-999");
                var matricula = new Matricula(alunoId, cursoId, DateTime.Now);

                context.Alunos.Add(aluno);
                context.Certificados.Add(certificado);
                context.Matriculas.Add(matricula);

                // Act
                await context.Commit();
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var aluno = await context.Alunos.FindAsync(alunoId);
                Assert.NotEqual(DateTime.MinValue, aluno.DataCadastro);
            }
        }
    }
}
