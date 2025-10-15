using MBA.EducaOn.Core.Data;
using MBA.EducaOn.GestaoAlunos.Data;
using MBA.EducaOn.GestaoConteudo.Data;
using MBA.EducaOn.Security.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MBA.EducaOn.Ioc.Configurations;

public static class DataContextConfig
{
    /// <summary>
    /// Configura e registra os pools de DbContext da aplicação para os contextos de Alunos, Conteúdo e Segurança.
    /// Utiliza SQLite em ambiente de desenvolvimento e SQL Server nos demais ambientes, conforme a configuração fornecida.
    /// Também registra os DbContexts com tempo de vida Scoped.
    /// </summary>
    /// <param name="builder">O <see cref="WebApplicationBuilder"/> utilizado para configurar os serviços e obter as configurações do ambiente.</param>
    public static void AddDataContextPool(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            var cnnLite = builder.Configuration.GetConnectionString("DefaultConnectionLite");

            //Bounded Context - Alunos
            builder.Services.AddDbContextPool<AlunoContext>(options =>
            {
                options.UseSqlite(cnnLite)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });

            //Bounded Context - Conteudo
            builder.Services.AddDbContextPool<ConteudoContext>(options =>
            {
                options.UseSqlite(cnnLite)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });

            //Security Context
            builder.Services.AddDbContextPool<SecurityDbContext>(options =>
            {
                options.UseSqlite(cnnLite)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });
        }
        else
        {
            var cnn = builder.Configuration.GetConnectionString("DefaultConnection");

            //Bounded Context - Alunos
            builder.Services.AddDbContextPool<AlunoContext>(options =>
            {
                options.UseSqlServer(cnn)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });

            //Bounded Context - Conteudo
            builder.Services.AddDbContextPool<ConteudoContext>(options =>
            {
                options.UseSqlServer(cnn)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });

            //Security Context
            builder.Services.AddDbContextPool<SecurityDbContext>(options =>
            {
                options.UseSqlServer(cnn)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });
        }

        builder.Services.AddScoped<AlunoContext>();
        builder.Services.AddScoped<ConteudoContext>();
        builder.Services.AddScoped<SecurityDbContext>();
        //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();>
    }
}
