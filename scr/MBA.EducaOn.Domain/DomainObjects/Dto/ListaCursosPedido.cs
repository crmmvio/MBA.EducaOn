namespace MBA.EducaOn.Core.DomainObjects.Dto;

public class ListaCursosPedido
{
    public Guid PedidoId { get; set; }
    public ICollection<Item> Itens { get; set; }
}

public class Item
{
    public Guid Id { get; set; }
    public string CursonNome{ get; set; }
}