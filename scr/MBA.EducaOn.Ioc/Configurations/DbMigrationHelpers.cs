using MBA.EducaOn.Core.Enumerators;
using MBA.EducaOn.Core.Extensions;
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

        var contextSecurity = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var contextConteudo = scope.ServiceProvider.GetRequiredService<ConteudoDbContext>();
        var contextAluno = scope.ServiceProvider.GetRequiredService<AlunoDbContext>();

        if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
        {
            await contextSecurity.Database.MigrateAsync();
            await contextConteudo.Database.MigrateAsync();
            await contextAluno.Database.MigrateAsync();

            await EnsureSeedRoles(contextSecurity);
            await EnsureSeedSecurity(userManager, contextSecurity, contextConteudo, contextAluno);
        }
    }

    private static async Task EnsureSeedRoles(SecurityDbContext contextIdentity)
    {
        // Verifica se já existem roles criadas
        if (await contextIdentity.Roles.AnyAsync())
            return;

        // Obtém todos os valores do enum TipoUsuario
        var tiposUsuario = Enum.GetValues(typeof(TipoUsuario)).Cast<TipoUsuario>();

        foreach (var tipoUsuario in tiposUsuario)
        {
            var roleName = tipoUsuario.GetDescription();
            var normalizedRoleName = roleName.ToUpperInvariant();
            if (!await contextIdentity.Roles.AnyAsync(r => r.NormalizedName == normalizedRoleName))
            {
                await contextIdentity.Roles.AddAsync(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = normalizedRoleName,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }
    }

    private static async Task EnsureSeedSecurity(UserManager<IdentityUser> userManager, SecurityDbContext contextSecurity, ConteudoDbContext contextConteudo, AlunoDbContext contextAluno)
    {
        var userId = Guid.NewGuid();
        var userEmail = "teste@crm.com";
        var userAdminEmail = "ADMINISTRADOR@CRM.COM";
                
        if (await userManager.FindByEmailAsync(userAdminEmail) == null)
        {
            var userAdmin = new IdentityUser
            {
                UserName = "Administrador",
                NormalizedUserName = "ADMINISTRADOR",
                Email = userAdminEmail,
                NormalizedEmail = userAdminEmail.ToUpperInvariant(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            var result = await userManager.CreateAsync(userAdmin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(userAdmin, TipoUsuario.Administrador.GetDescription().ToUpperInvariant());
            }
        }

        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var userAluno = new IdentityUser
            {
                Id = userId.ToString(),
                UserName = "AlunoTeste",
                NormalizedUserName = "ALUNOTESTE",
                Email = userEmail,
                NormalizedEmail = userEmail.ToUpperInvariant(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            var result = await userManager.CreateAsync(userAluno, "Aluno@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(userAluno, TipoUsuario.Aluno.GetDescription().ToUpperInvariant());
            }
        }

        // Cria um curso de teste
        var conteudoProgramatico = new ConteudoProgramatico("Conteudo Programatico Teste", 1, DateTime.Now);
        var curso = new Curso("Curso Teste", "Curso Teste Descricao", 100, 10, "Iniciante", "Teste", "Nenhum", conteudoProgramatico);
        await contextConteudo.Cursos.AddAsync(curso);

        // Adiciona algumas aulas ao curso
        var aluno = new Aluno(userId, "Aluno Teste", userEmail);
        aluno.AtualizarHistorico(new HistoricoAprendizado(aluno.Id, curso.Id, DateTime.Now));

        // Matricula o aluno no curso
        aluno.AdicionarMatricula(curso.Id);
        await contextAluno.Alunos.AddAsync(aluno);

        // Salva todas as mudanças nos contextos
        await contextConteudo.Commit();
        await contextAluno.Commit();
        await contextSecurity.SaveChangesAsync();
    }
}
