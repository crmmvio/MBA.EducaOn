using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Events;

public class VoucherAplicadoPedidoEvent : Event
{
    public Guid AlunoId { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid VoucherId { get; private set; }

    public VoucherAplicadoPedidoEvent(Guid alunoId, Guid pedidoId, Guid voucherId)
    {
        AggregateId = pedidoId;
        AlunoId = alunoId;
        PedidoId = pedidoId;
        VoucherId = voucherId;
    }
}
