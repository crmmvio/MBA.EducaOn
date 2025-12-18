using MBA.EducaOn.Core.Data;
using MBA.EducaOn.Pagamentos.Business;
using MBA.EducaOn.Pagamentos.Business.Interfaces;

namespace MBA.EducaOn.Pagamentos.Data.Repository;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly PagamentoDbContext _context;

    public PagamentoRepository(PagamentoDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;


    public void Adicionar(Pagamento pagamento)
    {
        _context.Pagamentos.Add(pagamento);
    }

    public void AdicionarTransacao(Transacao transacao)
    {
        _context.Transacoes.Add(transacao);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
