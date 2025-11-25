using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Data.EventSourcing;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MBA.EducaOn.GestaoAlunos.Application.Services;
using MBA.EducaOn.GestaoAlunos.Data.Repository;
using MBA.EducaOn.GestaoAlunos.Domain.Interfaces.Repositories;
using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.GestaoConteudo.Data.Repository;
using MBA.EducaOn.GestaoConteudo.Domain.Interfaces.Repositories;
using MBA.EducaOn.Vendas.Application.Commands;
using MediatR;
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
        var licenseKey = configuration.GetSection("mediator-license")?.Value;
        //MediatR
        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = licenseKey;
            cfg.RegisterServicesFromAssemblies(Assembly.Load("MBA.EducaOn.Core"));
        });

        //AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = licenseKey;
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
        },
        Assembly.Load("MBA.EducaOn.GestaoConteudo.Application"),
        Assembly.Load("MBA.EducaOn.GestaoAlunos.Application"));

        // Notifications
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        // Event Sourcing
        //services.AddSingleton<IEventStoreService, EventStoreService>();
        //services.AddSingleton<IEventSourcingRepository, EventSourcingRepository>();

        //BC - Conteudo
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<ICursoService, CursoService>();

        //BC- Alunos
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<IAlunoService, AlunoService>();

        //Vendas
        services.AddScoped<IRequestHandler<AdicionarItemPedidoCommand, bool>, PedidoCommandHandler>();
    }
}
