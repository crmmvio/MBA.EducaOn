using MBA.EducaOn.Vendas.Domain;

namespace MBA.EducaOn.Pagamentos.Business.Interfaces;

public interface IPagamentoCartaoCreditoFacade
{
    Transacao RealizarPagamento(Pedido pedido, Pagamento pagamento);
}
