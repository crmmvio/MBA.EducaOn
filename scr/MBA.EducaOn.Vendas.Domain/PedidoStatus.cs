namespace MBA.EducaOn.Vendas.Domain;

/// <summary>
/// Status do pedido
/// </summary>
public enum PedidoStatus
{
    /// <summary>
    /// Status de rascunho
    /// </summary>
    Rascunho = 0,

    /// <summary>
    /// Status de iniciado
    /// </summary>
    Iniciado = 1,

    /// <summary>
    /// Status de pago
    /// </summary>
    Pago = 4,

    /// <summary>
    /// Status de entregue
    /// </summary>
    Entregue = 5,

    /// <summary>
    /// Status de cancelado
    /// </summary>
    Cancelado = 6
}
