using MBA.EducaOn.Core.DomainObjects.Dto;

namespace MBA.EducaOn.Core.Messages.CommonMessages.IntegrationEvents;

public class PedidoEstoqueConfirmadoEvent : IntegrationEvent
{
    public Guid PedidoId { get; private set; }
    public Guid ClienteId { get; private set; }
    public decimal Total { get; private set; }
    public ListaCursosPedido CursosPedido { get; private set; }
    public string NomeCartao { get; private set; }
    public string NumeroCartao { get; private set; }
    public string ExpiracaoCartao { get; private set; }
    public string CvvCartao { get; private set; }

    public PedidoEstoqueConfirmadoEvent(Guid pedidoId, Guid clienteId, decimal total, ListaCursosPedido cursosPedido, string nomeCartao, string numeroCartao, string expiracaoCartao, string cvvCartao)
    {
        AggregateId = pedidoId;
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Total = total;
        CursosPedido = cursosPedido;
        NomeCartao = nomeCartao;
        NumeroCartao = numeroCartao;
        ExpiracaoCartao = expiracaoCartao;
        CvvCartao = cvvCartao;
    }
}
