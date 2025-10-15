using MBA.EducaOn.GestaoAlunos.Data;
using MBA.EducaOn.GestaoAlunos.Domain;
using MBA.EducaOn.GestaoConteudo.Data;
using MBA.EducaOn.GestaoConteudo.Domain;
using MBA.EducaOn.Security.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MBA.EducaOn.Ioc.Configurations;

/// <summary>
/// Métodos de extensão para aplicar helpers de migração de banco de dados em um <see cref="WebApplication"/>.
/// </summary>
public static class DbMigrationHelperExtension
{
    /// <summary>
    /// Aplica o helper de migração de banco de dados à instância especificada de <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="app">A instância da aplicação web.</param>
    public static void UseDbMigrationHelper(this WebApplication app)
    {
        DbMigrationHelpers.EnsureSeedData(app).Wait();
    }
}

/// <summary>
/// Fornece métodos auxiliares para aplicar migrações de banco de dados e popular dados iniciais.
/// </summary>
public static class DbMigrationHelpers
{
    /// <summary>
    /// Garante que o banco de dados seja populado com dados iniciais usando o <see cref="WebApplication"/> especificado.
    /// </summary>
    /// <param name="serviceScope">A instância da aplicação web.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public static async Task EnsureSeedData(WebApplication serviceScope)
    {
        var services = serviceScope.Services.CreateScope().ServiceProvider;
        await EnsureSeedData(services);
    }

    /// <summary>
    /// Garante que o banco de dados seja populado com dados iniciais usando o <see cref="IServiceProvider"/> especificado.
    /// </summary>
    /// <param name="serviceProvider">O provedor de serviços.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var context = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
        var contextConteudo = scope.ServiceProvider.GetRequiredService<ConteudoContext>();
        var contextAluno = scope.ServiceProvider.GetRequiredService<AlunoContext>();

        if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
        {
            await context.Database.MigrateAsync();
            await contextConteudo.Database.MigrateAsync();
            await contextAluno.Database.MigrateAsync();

            await EnsureSeedSecurity(context, contextConteudo, contextAluno);
        }
    }

    private static async Task EnsureSeedSecurity(SecurityDbContext context, ConteudoContext contextConteudo, AlunoContext contextAluno)
    {

        if (!await contextConteudo.Cursos.AnyAsync())
        {
            var userId = Guid.NewGuid();
            var userEmail = "teste@crm.com";
            var conteudoProgramatico = new ConteudoProgramatico("Conteudo Programatico Teste", 1, DateTime.Now);
            var curso = new Curso("Curso Teste", "Curso Teste Descricao", 100, 10, "Iniciante", "Teste", "Nenhum", conteudoProgramatico);

            await contextConteudo.Cursos.AddAsync(curso);

            await context.Users.AddAsync(new IdentityUser
            {
                Id = userId.ToString(),
                UserName = "AlunoTeste",
                NormalizedUserName = "ALUNOTESTE",
                Email = userEmail,
                NormalizedEmail = "TESTE@CRM.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEI8VDADrqtpXkqh0aUjERlWI1OPHO77GbMmNYMheOGW4PpoSB3HdROpkrVTk9wyefw==",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            });

            var aluno = new Aluno(userId, "Aluno Teste", userEmail);
            aluno.AtualizarHistorico(new HistoricoAprendizado(aluno.Id, curso.Id, DateTime.Now));

            aluno.AdicionarMatricula(curso.Id);
            await contextAluno.Alunos.AddAsync(aluno);

            await contextConteudo.Commit();
            await contextAluno.Commit();
            await context.SaveChangesAsync();
        }
    }
}
