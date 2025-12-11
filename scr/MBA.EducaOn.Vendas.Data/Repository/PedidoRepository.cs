using MBA.EducaOn.Core.Data;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MBA.EducaOn.Vendas.Data.Repository;

public class PedidoRepository : IPedidoRepository
{
    private readonly VendasDbContext _context;
    private bool disposedValue;

    public PedidoRepository(VendasDbContext context)
    {
        _context = context;
    }

    ///<inheritdoc/>
    public IUnitOfWork UnitOfWork => _context;

    ///<inheritdoc/>
    public async Task<Pedido> ObterPorId(Guid id)
    {
        return await _context.Pedidos.FindAsync(id);
    }

    ///<inheritdoc/>
    public async Task<int> ObterProximoCodigo()
    {
        // Supondo que o código do pedido seja um número sequencial armazenado em uma coluna "Codigo"
        var ultimoPedido = await _context.Pedidos
                                         .OrderByDescending(p => p.Codigo)
                                         .FirstOrDefaultAsync();
        return (ultimoPedido != null) ? ultimoPedido.Codigo + 1 : 1;
    }

    ///<inheritdoc/>
    public async Task<IEnumerable<Pedido>> ObterListaPorAlunoId(Guid clienteId)
    {
        return await _context.Pedidos
                             .AsNoTracking()
                             .Where(p => p.AlunoId == clienteId)
                             .ToListAsync();
    }

    ///<inheritdoc/>
    public async Task<Pedido> ObterPedidoRascunhoPorAlunoId(Guid alunoId)
    {
        var pedido = await _context.Pedidos
                                 .FirstOrDefaultAsync(p => p.AlunoId == alunoId &&
                                                           p.PedidoStatus == PedidoStatus.Rascunho);
        if(pedido == null) return null!;

        await _context.Entry(pedido)
                      .Collection(p => p.PedidoItems)
                      .LoadAsync();

        if(pedido.VoucherId != null)
        {
            await _context.Entry(pedido)
                          .Reference(p => p.Voucher)
                          .LoadAsync();
        }

        return pedido;
    }

    ///<inheritdoc/>
    public async Task<PedidoItem> ObterItemPorId(Guid id)
    {
        return await _context.PedidoItems.FindAsync(id);
    }

    ///<inheritdoc/>
    public async Task<PedidoItem> ObterItemPorPedido(Guid pedidoId, Guid cursoId)
    {
        return await _context.PedidoItems
                             .FirstOrDefaultAsync(pi => pi.PedidoId == pedidoId && pi.CursoId == cursoId);
    }

    ///<inheritdoc/>
    public async Task<Voucher> ObterVoucherPorCodigo(string codigo)
    {
        return await _context.Vouchers
                             .FirstOrDefaultAsync(v => v.Codigo == codigo);
    }

    ///<inheritdoc/>
    public void Adicionar(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
    }

    ///<inheritdoc/>
    public void AdicionarItem(PedidoItem pedidoItem)
    {
        _context.PedidoItems.Add(pedidoItem);
    }

    ///<inheritdoc/>
    public void Atualizar(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
    }

    ///<inheritdoc/>
    public void AtualizarItem(PedidoItem pedidoItem)
    {
        _context.PedidoItems.Update(pedidoItem);
    }

    ///<inheritdoc/>
    public void RemoverItem(PedidoItem pedidoItem)
    {
        _context.PedidoItems.Remove(pedidoItem);
    }

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
