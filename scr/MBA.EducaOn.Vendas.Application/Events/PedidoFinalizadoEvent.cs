using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Events;

public class PedidoFinalizadoEvent : Event
{
    public Guid PedidoId { get; private set; }

    public PedidoFinalizadoEvent(Guid pedidoId)
    {
        PedidoId = pedidoId;
        AggregateId = pedidoId;
    }
}
