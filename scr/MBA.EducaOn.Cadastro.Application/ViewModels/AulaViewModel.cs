namespace MBA.EducaOn.GestaoConteudo.Application.ViewModels;

public class AulaViewModel
{
    public Guid Id { get; set; }
    public string Codigo { get;  set; } 
    public string Titulo { get;  set; }
    public string Descricao { get;  set; }
    public int Ordem { get;  set; }
    public Guid CursoId { get;  set; }
    public DateTime DataCadastro { get;  set; }
    public bool Ativo { get;  set; }
}
