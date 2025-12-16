using MBA.EducaOn.Vendas.Application.Queries.ViewModels;

namespace MBA.EducaOn.Vendas.Application.Queries;

/// <summary>
/// Interface que define consultas (queries) relacionadas a pedidos e carrinho de compras.
/// </summary>
public interface IPedidoQueries
{
    /// <summary>
    /// Obtém o carrinho de compras do aluno identificado por <paramref name="alunoId"/>.
    /// </summary>
    /// <param name="alunoId">Identificador único do cliente (aluno) cujo carrinho será recuperado.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado contém um <see cref="CarrinhoViewModel"/> representando o carrinho do cliente.</returns>
    Task<CarrinhoViewModel> ObterCarrinhoAluno(Guid alunoId);

    /// <summary>
    /// Obtém a lista de pedidos do cliente identificado por <paramref name="alunoId"/>.
    /// </summary>
    /// <param name="alunoId">Identificador único do aluno cujos pedidos serão recuperados.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado contém uma coleção de <see cref="PedidoViewModel"/> com os pedidos do cliente.</returns>
    Task<IEnumerable<PedidoViewModel>> ObterPedidosAluno(Guid alunoId);
}
