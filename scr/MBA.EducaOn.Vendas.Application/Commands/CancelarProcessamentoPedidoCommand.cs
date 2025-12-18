using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class CancelarProcessamentoPedidoCommand : Command
{
    public Guid PedidoId { get; private set; }
    public Guid AlunoId { get; private set; }

    public CancelarProcessamentoPedidoCommand(Guid pedidoId, Guid alunoId)
    {
        AggregateId = pedidoId;
        PedidoId = pedidoId;
        AlunoId = alunoId;
    }
}
