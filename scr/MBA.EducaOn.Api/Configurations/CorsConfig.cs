namespace MBA.EducaOn.Api.Configurations;

/// <summary>
/// Fornece métodos de extensão para configurar políticas de CORS na aplicação.
/// </summary>
public static class CorsConfig
{
    /// <summary>
    /// Adiciona a configuração de CORS ao <see cref="WebApplicationBuilder"/> especificado.
    /// </summary>
    /// <param name="builder">O <see cref="WebApplicationBuilder"/> a ser configurado.</param>
    /// <returns>O <see cref="WebApplicationBuilder"/> configurado.</returns>
    public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Development",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());

            options.AddPolicy("Production",
                 builder => builder.WithOrigins("https://localhost:9000")
                                   .WithMethods("POST")
                                  .AllowAnyHeader());
        });

        return builder;
    }
}
