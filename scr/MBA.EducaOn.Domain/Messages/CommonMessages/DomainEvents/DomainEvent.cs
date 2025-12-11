using MediatR;

namespace MBA.EducaOn.Core.Messages.CommonMessages.DomainEvents;

public abstract class DomainEvent : Event, INotification
{
    public DateTime Timestamp { get; private set; }

    protected DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
        Timestamp = DateTime.Now;
    }
}
