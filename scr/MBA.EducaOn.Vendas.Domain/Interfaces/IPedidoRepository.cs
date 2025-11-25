using MBA.EducaOn.Core.Data;

namespace MBA.EducaOn.Vendas.Domain.Interfaces;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido> ObterPorId(Guid id);
    Task<IEnumerable<Pedido>> ObterListaPorAlunoId(Guid clienteId);
    Task<Pedido> ObterPedidoRascunhoPorAlunoId(Guid alunoId);
    void Adicionar(Pedido pedido);
    void Atualizar(Pedido pedido);

    Task<PedidoItem> ObterItemPorId(Guid id);
    Task<PedidoItem> ObterItemPorPedido(Guid pedidoId, Guid cursoId);
    void AdicionarItem(PedidoItem pedidoItem);
    void AtualizarItem(PedidoItem pedidoItem);
    void RemoverItem(PedidoItem pedidoItem);

    Task<Voucher> ObterVoucherPorCodigo(string codigo);
}