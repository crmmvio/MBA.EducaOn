using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.Vendas.Domain;

public class PedidoItem : Entity
{
    protected PedidoItem() { }

    public PedidoItem(Guid cursoId, string cursoNome, decimal valorUnitario)
    {
        CursoId = cursoId;
        CursonNome = cursoNome;
        ValorUnitario = valorUnitario;
    }

    public Guid PedidoId { get; private set; }
    public Guid CursoId { get; private set; }
    public string CursonNome { get; private set; }
    public decimal ValorUnitario { get; private set; }

    public Pedido Pedido { get; set; }

    internal void AssociarPedido(Guid pedidoId)
    {
        PedidoId = pedidoId;
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants
    public const int NomeCursoMaxLength = 200;
    #endregion
}