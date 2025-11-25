using MBA.EducaOn.GestaoConteudo.Data.Repository;
using MBA.EducaOn.GestaoConteudo.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MBA.EducaOn.GestaoConteudo.Data.Test;

public class CursoRepositoryTests
{
    private ConteudoDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ConteudoDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ConteudoDbContext(options);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarCursoComAulasQuandoExistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("conteudo", 1, DateTime.Now);
        var curso = new Curso("Curso A", "Descricao", 10m, 5, "Publico", "Objetivo", "Requisitos", conteudo);
        var aula = new Aula(curso.Id, "C1", "Titulo", "Desc", 1);
        curso.AdicionarAula(aula);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.Add(curso);
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var result = await repo.ObterPorIdAsync(curso.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(curso.Id, result.Id);
            Assert.NotNull(result.Aulas);
            Assert.Single(result.Aulas);
            Assert.Equal(aula.Id, result.Aulas.First().Id);
        }
    }

    [Fact]
    public async Task ObterPorAulaIdAsync_DeveRetornarCursoQueContemAula()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("conteudo", 1, DateTime.Now);
        var curso = new Curso("Curso B", "Descricao", 10m, 5, "Publico", "Objetivo", "Requisitos", conteudo);
        var aula = new Aula(curso.Id, "C2", "Titulo2", "Desc2", 2);
        curso.AdicionarAula(aula);

        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            context.Cursos.Add(curso);
            await repo.UnitOfWork.Commit();
        }

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var result = await repo.ObterPorAulaIdAsync(aula.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(curso.Id, result.Id);
            Assert.Contains(result.Aulas, a => a.Id == aula.Id);
        }
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosOsCursos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var c1 = new Curso("C1", "D1", 1m, 1, "p", "o", "r", conteudo);
        var c2 = new Curso("C2", "D2", 2m, 2, "p", "o", "r", conteudo);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.AddRange(c1, c2);
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var list = await repo.ObterTodosAsync();

            // Assert
            Assert.NotNull(list);
            Assert.Equal(2, list.Count());
            Assert.Contains(list, e => e.Id == c1.Id);
            Assert.Contains(list, e => e.Id == c2.Id);
        }
    }

    [Fact]
    public async Task ExistAsync_ByGuid_DeveRetornarTrueQuandoExistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var curso = new Curso("CExist", "D", 1m, 1, "p", "o", "r", conteudo);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.Add(curso);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var exists = await repo.ExistAsync(curso.Id);
            Assert.True(exists);

            var notExists = await repo.ExistAsync(Guid.NewGuid());
            Assert.False(notExists);
        }
    }

    [Fact]
    public async Task ExistAsync_ByNome_DeveRetornarTrueQuandoExistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var curso = new Curso("NomeExistente", "D", 1m, 1, "p", "o", "r", conteudo);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.Add(curso);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var exists = await repo.ExistAsync("NomeExistente");
            Assert.True(exists);

            var notExists = await repo.ExistAsync("OutroNome");
            Assert.False(notExists);
        }
    }

    [Fact]
    public async Task Adicionar_DevePersistirCursoAoCommit()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var curso = new Curso("NovoCurso", "D", 1m, 1, "p", "o", "r", conteudo);

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            repo.Adicionar(curso);
            var committed = await repo.UnitOfWork.Commit();

            // Assert commit returned true
            Assert.True(committed);
        }

        // Validate persisted
        await using (var context = CreateContext(dbName))
        {
            var persisted = await context.Cursos.FindAsync(curso.Id);
            Assert.NotNull(persisted);
            Assert.Equal("NovoCurso", persisted.Nome);
        }
    }

    [Fact]
    public async Task Atualizar_DeveAplicarMudancasECommitPersistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var curso = new Curso("ToUpdate", "D", 1m, 1, "p", "o", "r", conteudo);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.Add(curso);
            await context.SaveChangesAsync();
        }

        // Act - change a property via reflection and update
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            var fetched = await context.Cursos.FirstAsync(e => e.Id == curso.Id);

            // use reflection to call the private setter of Nome (or the private method if needed)
            var prop = typeof(Curso).GetProperty(nameof(Curso.Nome), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var setMethod = prop.GetSetMethod(true);
            setMethod.Invoke(fetched, new object[] { "Atualizado" });

            repo.Atualizar(fetched);
            var committed = await repo.UnitOfWork.Commit();
            Assert.True(committed);
        }

        // Assert persisted change
        await using (var context = CreateContext(dbName))
        {
            var fromDb = await context.Cursos.FindAsync(curso.Id);
            Assert.Equal("Atualizado", fromDb.Nome);
        }
    }

    [Fact]
    public async Task Deletar_DeveRemoverCursoAoCommit()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var conteudo = new ConteudoProgramatico("c", 1, DateTime.Now);
        var curso = new Curso("ToDelete", "D", 1m, 1, "p", "o", "r", conteudo);

        await using (var context = CreateContext(dbName))
        {
            context.Cursos.Add(curso);
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);
            await repo.Deletar(curso.Id);
            var committed = await repo.UnitOfWork.Commit();
            Assert.True(committed);
        }

        // Assert removed
        await using (var context = CreateContext(dbName))
        {
            var fromDb = await context.Cursos.FindAsync(curso.Id);
            Assert.Null(fromDb);
        }
    }

    [Fact]
    public void Dispose_NaoDeveLancarExcecao()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var context = CreateContext(dbName))
        {
            var repo = new CursoRepository(context);

            // Act / Assert - apenas chamar Dispose não deve lançar
            repo.Dispose();
        }
    }
}