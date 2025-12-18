using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class CancelarProcessamentoPedidoNotificarAlunoCommand : Command
{
    public Guid PedidoId { get; private set; }
    public Guid AlunoId { get; private set; }

    public CancelarProcessamentoPedidoNotificarAlunoCommand(Guid pedidoId, Guid alunoId)
    {
        AggregateId = pedidoId;
        PedidoId = pedidoId;
        AlunoId = alunoId;
    }
}
