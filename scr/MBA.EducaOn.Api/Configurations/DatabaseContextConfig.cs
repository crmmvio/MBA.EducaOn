using MBA.EducaOn.Security.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.Api.Configurations;

/// <summary>
/// Fornece métodos de extensão para configurar o contexto de banco de dados na aplicação.
/// </summary>
public static class DatabaseContextConfig
{
    /// <summary>
    /// Adiciona e configura o <see cref="SecurityDbContext"/> para a aplicação.
    /// </summary>
    /// <param name="builder">O <see cref="WebApplicationBuilder"/> a ser configurado.</param>
    /// <returns>O <see cref="WebApplicationBuilder"/> configurado.</returns>
    public static WebApplicationBuilder AddDatabaseContextConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<SecurityDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        return builder;
    }
}
