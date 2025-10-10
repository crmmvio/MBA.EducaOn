using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MBA.EducaOn.Ioc;

public static class DependencyInjectionRegisterExtensions
{
    /// <summary>
    /// Registra os serviços e dependências essenciais da aplicação, como MediatR, repositórios, serviços,
    /// Event Sourcing, configuração de contexto de dados, identidade e mapeamento de objetos.
    /// Deve ser chamado durante a configuração do container de injeção de dependência.
    /// </summary>
    /// <param name="services">A coleção de serviços onde as dependências serão registradas.</param>
    /// <param name="configuration">A configuração da aplicação utilizada para registrar serviços dependentes de configuração.</param>
    public static void AddDependencyInjectionRegister(this IServiceCollection services, IConfiguration configuration)
    {
        //MediatR
        //services.AddMediatR(cfg => {
        //    cfg.LicenseKey = configuration.GetSection("mediator-license")?.Value;
        //    cfg.RegisterServicesFromAssemblies(Assembly.Load("MBA.EducaOn.Core"));
        //});

        //Repository

        //Service
        
        //EventSourcing

        //Configurations.DataContextSetup.AddDataContextPool(services, configuration);

        //Configurations.RepositorySetup.AddRepositories(services);
        //Configurations.ServiceSetup.AddServices(services);
        //Configurations.IdentitySetup.AddIdentity(services, configuration);
        //Configurations.AutoMapperSetup.AddAutoMapper(services);
        //Configurations.MediatRSetup.AddMediatR(services);
    }
}
