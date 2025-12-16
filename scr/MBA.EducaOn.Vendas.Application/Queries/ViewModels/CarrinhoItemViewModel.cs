namespace MBA.EducaOn.Vendas.Application.Queries.ViewModels;

public class CarrinhoItemViewModel
{
    public Guid CursoId { get; set; }
    public string CursoNome { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}
