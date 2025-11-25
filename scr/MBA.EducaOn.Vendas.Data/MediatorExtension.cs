using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.Vendas.Data;

public static class MediatorExtension
{
    public static async Task PublicarEventos(this IMediatorHandler mediator, VendasDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Notificacoes)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.LimparEventos());

        var tasks = domainEvents
            .Select(async (domainEvent) => {
                await mediator.PublicarEvento(domainEvent);
            });

        await Task.WhenAll(tasks);
    }
}
