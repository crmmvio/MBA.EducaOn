using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MBA.EducaOn.Api.Configurations;

/// <summary>
/// Fornece métodos de extensão para configurar o Swagger na API.
/// </summary>
public static class SwaggerConfig
{
    /// <summary>
    /// Adiciona a configuração do Swagger ao <see cref="WebApplicationBuilder"/> especificado.
    /// </summary>
    /// <param name="builder">O builder da aplicação web.</param>
    /// <returns>O <see cref="WebApplicationBuilder"/> atualizado.</returns>
    public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "API EducaOn",
                Version = "v1",
                Description = "Projeto MBA: API - EducaOn",
                Contact = new OpenApiContact()
                {
                    Name = "Cleber Roberto Movio",
                    Email = "crmmvio@gmail.com"
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

        });

        return builder;
    }

}
