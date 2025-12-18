using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class FinalizarPedidoCommand : Command
{
    public Guid PedidoId { get; private set; }
    public Guid AlunoId { get; private set; }

    public FinalizarPedidoCommand(Guid pedidoId, Guid alunoId)
    {
        AggregateId = pedidoId;
        PedidoId = pedidoId;
        AlunoId = alunoId;
    }
}
