using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Events;

public class PedidoRascunhoIniciadoEvent : Event
{
    public Guid AlunoId { get; private set; }
    public Guid PedidoId { get; private set; }

    public PedidoRascunhoIniciadoEvent(Guid alunoId, Guid pedidoId)
    {
        AggregateId = pedidoId;
        AlunoId = alunoId;
        PedidoId = pedidoId;
    }
}
