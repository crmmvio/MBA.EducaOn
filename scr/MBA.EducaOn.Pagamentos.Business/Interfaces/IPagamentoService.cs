using MBA.EducaOn.Core.DomainObjects.Dto;

namespace MBA.EducaOn.Pagamentos.Business.Interfaces;

public interface IPagamentoService
{
    Task<Transacao> RealizarPagamentoPedido(PagamentoPedido pagamentoPedido);
}
