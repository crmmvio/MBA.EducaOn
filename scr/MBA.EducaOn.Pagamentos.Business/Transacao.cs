using MBA.EducaOn.Core.DomainObjects;
using MBA.EducaOn.Pagamentos.Business.Enumerators;

namespace MBA.EducaOn.Pagamentos.Business;

public class Transacao : Entity
{
    public Guid PedidoId { get; set; }
    public Guid PagamentoId { get; set; }
    public decimal Total { get; set; }
    public StatusTransacao StatusTransacao { get; set; }

    // EF. Rel.
    public Pagamento Pagamento { get; set; }
}
