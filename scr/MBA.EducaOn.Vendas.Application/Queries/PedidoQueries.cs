using MBA.EducaOn.Vendas.Application.Queries.ViewModels;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;

namespace MBA.EducaOn.Vendas.Application.Queries;

///<inheritdoc />
public class PedidoQueries : IPedidoQueries
{
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoQueries(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    ///<inheritdoc />
    public async Task<CarrinhoViewModel> ObterCarrinhoAluno(Guid alunoId)
    {
        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(alunoId);
        if (pedido == null) return null;

        var carrinho = new CarrinhoViewModel
        {
            AlunoId = pedido.AlunoId,
            ValorTotal = pedido.ValorTotal,
            PedidoId = pedido.Id,
            ValorDesconto = pedido.Desconto,
            SubTotal = pedido.Desconto + pedido.ValorTotal
        };

        if (pedido.VoucherId != null)
        {
            carrinho.VoucherCodigo = pedido.Voucher.Codigo;
        }

        foreach (var item in pedido.PedidoItems)
        {
            carrinho.Items.Add(new CarrinhoItemViewModel
            {
                CursoId = item.CursoId,
                CursoNome = item.CursonNome,
                ValorUnitario = item.ValorUnitario,
                ValorTotal = item.ValorUnitario
            });
        }

        return carrinho;
    }

    ///<inheritdoc />
    public async Task<IEnumerable<PedidoViewModel>> ObterPedidosAluno(Guid clienteId)
    {
        var pedidos = await _pedidoRepository.ObterListaPorAlunoId(clienteId);

        pedidos = pedidos.Where(p => p.PedidoStatus == PedidoStatus.Pago 
                                  || p.PedidoStatus == PedidoStatus.Cancelado)
                         .OrderByDescending(p => p.Codigo);

        if (!pedidos.Any()) return null;

        var pedidosView = new List<PedidoViewModel>();

        foreach (var pedido in pedidos)
        {
            pedidosView.Add(new PedidoViewModel
            {
                Id = pedido.Id,
                ValorTotal = pedido.ValorTotal,
                PedidoStatus = (int)pedido.PedidoStatus,
                Codigo = pedido.Codigo,
                DataCadastro = pedido.DataCadastro
            });
        }

        return pedidosView;
    }
}
