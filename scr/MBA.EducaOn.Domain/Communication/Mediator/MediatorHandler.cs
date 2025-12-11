using MBA.EducaOn.Core.Data.EventSourcing;
using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Core.Messages.CommonMessages.DomainEvents;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MediatR;

namespace MBA.EducaOn.Core.Communication.Mediator;

///<inheritdoc/>
public class MediatorHandler : IMediatorHandler
{
    private readonly IMediator _mediator;
    private readonly IEventSourcingRepository _eventSourcingRepository;

    public MediatorHandler(IMediator mediator,
                           IEventSourcingRepository eventSourcingRepository)
    {
        _mediator = mediator;
        _eventSourcingRepository = eventSourcingRepository;
    }

    ///<inheritdoc/>
    public async Task<bool> EnviarComando<T>(T comando) where T : Command
    {
        return await _mediator.Send(comando);
    }

    ///<inheritdoc/>
    public async Task PublicarEvento<T>(T evento) where T : Event
    {
        await _mediator.Publish(evento);
        await _eventSourcingRepository.SalvarEvento(evento);

    }

    ///<inheritdoc/>
    public async Task PublicarNotificacao<T>(T notificacao) where T : DomainNotification
    {
        await _mediator.Publish(notificacao);
    }

    ///<inheritdoc/>
    public async Task PublicarDomainEvent<T>(T notificacao) where T : DomainEvent
    {
        await _mediator.Publish(notificacao);
    }
}
