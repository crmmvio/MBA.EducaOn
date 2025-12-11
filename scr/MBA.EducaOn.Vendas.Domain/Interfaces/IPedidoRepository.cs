using MBA.EducaOn.Core.Data;

namespace MBA.EducaOn.Vendas.Domain.Interfaces;

/// <summary>
/// Repositório responsável pelas operações de persistência e consulta relacionadas ao agregado <see cref="Pedido"/>.
/// </summary>
public interface IPedidoRepository : IRepository<Pedido>
{
    /// <summary>
    /// Recupera um pedido pelo seu identificador único.
    /// </summary>
    /// <param name="id">Identificador único do pedido.</param>
    /// <returns>Uma tarefa que contém o <see cref="Pedido"/> encontrado ou <c>null</c> se não existir.</returns>
    Task<Pedido> ObterPorId(Guid id);

    /// <summary>
    /// Obtém o próximo código sequencial para um novo pedido.
    /// </summary>
    Task<int> ObterProximoCodigo();

    /// <summary>
    /// Recupera a lista de pedidos de um aluno/cliente.
    /// </summary>
    /// <param name="clienteId">Identificador do aluno/cliente.</param>
    /// <returns>Uma tarefa que contém uma coleção de pedidos associados ao aluno.</returns>
    Task<IEnumerable<Pedido>> ObterListaPorAlunoId(Guid clienteId);

    /// <summary>
    /// Recupera o pedido em rascunho (não finalizado) de um aluno pelo seu identificador.
    /// </summary>
    /// <param name="alunoId">Identificador do aluno.</param>
    /// <returns>Uma tarefa que contém o pedido em rascunho ou <c>null</c> se não existir.</returns>
    Task<Pedido> ObterPedidoRascunhoPorAlunoId(Guid alunoId);

    /// <summary>
    /// Adiciona um novo pedido ao repositório (prepara para persistência).
    /// </summary>
    /// <param name="pedido">Instância do pedido a ser adicionada.</param>
    void Adicionar(Pedido pedido);

    /// <summary>
    /// Atualiza um pedido existente no repositório (prepara mudanças para persistência).
    /// </summary>
    /// <param name="pedido">Instância do pedido com as alterações aplicadas.</param>
    void Atualizar(Pedido pedido);

    /// <summary>
    /// Recupera um item de pedido pelo seu identificador único.
    /// </summary>
    /// <param name="id">Identificador único do item do pedido.</param>
    /// <returns>Uma tarefa que contém o <see cref="PedidoItem"/> encontrado ou <c>null</c> se não existir.</returns>
    Task<PedidoItem> ObterItemPorId(Guid id);

    /// <summary>
    /// Recupera um item do pedido pelo identificador do pedido e do curso associado.
    /// </summary>
    /// <param name="pedidoId">Identificador do pedido.</param>
    /// <param name="cursoId">Identificador do curso.</param>
    /// <returns>Uma tarefa que contém o <see cref="PedidoItem"/> correspondente ou <c>null</c> se não existir.</returns>
    Task<PedidoItem> ObterItemPorPedido(Guid pedidoId, Guid cursoId);

    /// <summary>
    /// Adiciona um item ao pedido (prepara para persistência).
    /// </summary>
    /// <param name="pedidoItem">Instância do item do pedido a ser adicionada.</param>
    void AdicionarItem(PedidoItem pedidoItem);

    /// <summary>
    /// Atualiza um item de pedido existente (prepara mudanças para persistência).
    /// </summary>
    /// <param name="pedidoItem">Instância do item do pedido com as alterações aplicadas.</param>
    void AtualizarItem(PedidoItem pedidoItem);

    /// <summary>
    /// Remove um item de pedido (prepara remoção para persistência).
    /// </summary>
    /// <param name="pedidoItem">Instância do item do pedido a ser removida.</param>
    void RemoverItem(PedidoItem pedidoItem);

    /// <summary>
    /// Recupera um voucher pelo seu código.
    /// </summary>
    /// <param name="codigo">Código do voucher.</param>
    /// <returns>Uma tarefa que contém o <see cref="Voucher"/> correspondente ou <c>null</c> se não existir.</returns>
    Task<Voucher> ObterVoucherPorCodigo(string codigo);
}