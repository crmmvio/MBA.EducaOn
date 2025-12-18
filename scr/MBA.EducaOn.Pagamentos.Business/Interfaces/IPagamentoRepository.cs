using MBA.EducaOn.Core.Data;

namespace MBA.EducaOn.Pagamentos.Business.Interfaces;

public interface IPagamentoRepository : IRepository<Pagamento>
{
    void Adicionar(Pagamento pagamento);

    void AdicionarTransacao(Transacao transacao);
}
