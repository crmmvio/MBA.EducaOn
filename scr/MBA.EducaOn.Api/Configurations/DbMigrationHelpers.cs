using MBA.EducaOn.Security.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.Api.Configurations;

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

        if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
        {
            await context.Database.MigrateAsync();

            await EnsureSeedProducts(context);
        }
    }

    private static async Task EnsureSeedProducts(SecurityDbContext context)
    {
        var alunoId = Guid.NewGuid().ToString();

        await context.Users.AddAsync(new IdentityUser
        {
            Id = alunoId,
            UserName = "teste@crm.com",
            NormalizedUserName = "TESTE@CRM.COM",
            Email = "teste@crm.com",
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

        await context.SaveChangesAsync();
    }
}
