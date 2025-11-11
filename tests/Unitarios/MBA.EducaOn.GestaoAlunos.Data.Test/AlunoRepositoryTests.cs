using MBA.EducaOn.GestaoAlunos.Data.Repository;
using MBA.EducaOn.GestaoAlunos.Domain;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.GestaoAlunos.Data.Test;

public class AlunoRepositoryTests
{
    private AlunoDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AlunoDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AlunoDbContext(options);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarAlunoQuandoExistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();
        var aluno = new Aluno(id, "Nome Teste", "teste@mail.com");

        await using (var context = CreateContext(dbName))
        {
            context.Alunos.Add(aluno);
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = CreateContext(dbName))
        {
            var repo = new AlunoRepository(context);
            var resultado = await repo.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(id, resultado.Id);
            Assert.Equal("Nome Teste", resultado.Nome);
            Assert.Equal("teste@mail.com", resultado.Email);
        }
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosOsAlunos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var aluno1 = new Aluno(Guid.NewGuid(), "A1", "a1@mail.com");
        var aluno2 = new Aluno(Guid.NewGuid(), "A2", "a2@mail.com");

        await using (var context = CreateContext(dbName))
        {
            context.Alunos.AddRange(aluno1, aluno2);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        await using (var context = CreateContext(dbName))
        {
            var repo = new AlunoRepository(context);
            var lista = await repo.ObterTodosAsync();

            Assert.NotNull(lista);
            Assert.Equal(2, lista.Count());
            Assert.Contains(lista, a => a.Id == aluno1.Id);
            Assert.Contains(lista, a => a.Id == aluno2.Id);
        }
    }

    [Fact]
    public async Task Adicionar_DeveInserirAlunoENoCommitPersistir()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var aluno = new Aluno(Guid.NewGuid(), "Novo", "novo@mail.com");

        await using (var context = CreateContext(dbName))
        {
            var repo = new AlunoRepository(context);

            // Act
            repo.Adicionar(aluno);
            var commitResult = await repo.UnitOfWork.Commit();

            // Assert
            Assert.True(commitResult);
        }

        await using (var context = CreateContext(dbName))
        {
            var existente = await context.Alunos.FindAsync(aluno.Id);
            Assert.NotNull(existente);
            Assert.Equal("Novo", existente.Nome);
        }
    }

    [Fact]
    public async Task Atualizar_DeveAlterarAlunoENoCommitPersistirMudancas()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();
        var original = new Aluno(id, "Original", "orig@mail.com");

        await using (var context = CreateContext(dbName))
        {
            context.Alunos.Add(original);
            await context.SaveChangesAsync();
        }

        // Act
        var updated = new Aluno(id, "Atualizado", "orig@mail.com");

        await using (var context = CreateContext(dbName))
        {
            var repo = new AlunoRepository(context);
            repo.Atualizar(updated);
            var commitResult = await repo.UnitOfWork.Commit();

            // Assert commit result
            Assert.True(commitResult);
        }

        await using (var context = CreateContext(dbName))
        {
            var fromDb = await context.Alunos.FindAsync(id);
            Assert.NotNull(fromDb);
            Assert.Equal("Atualizado", fromDb.Nome);
        }
    }

    [Fact]
    public void Dispose_NaoDeveLancarExcecao()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        using (var context = CreateContext(dbName))
        {
            var repo = new AlunoRepository(context);

            // Act / Assert - apenas chamar Dispose não deve lançar
            repo.Dispose();
        }
    }
}