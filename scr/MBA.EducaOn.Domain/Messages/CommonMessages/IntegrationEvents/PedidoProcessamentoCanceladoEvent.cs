using MBA.EducaOn.Core.DomainObjects.Dto;

namespace MBA.EducaOn.Core.Messages.CommonMessages.IntegrationEvents;

public class PedidoProcessamentoCanceladoEvent : IntegrationEvent
{
    public Guid PedidoId { get; private set; }
    public Guid ClienteId { get; private set; }
    public ListaCursosPedido CursosPedido { get; private set; }

    public PedidoProcessamentoCanceladoEvent(Guid pedidoId, Guid clienteId, ListaCursosPedido cursosPedido)
    {
        AggregateId = pedidoId;
        PedidoId = pedidoId;
        ClienteId = clienteId;
        CursosPedido = cursosPedido;
    }
}
