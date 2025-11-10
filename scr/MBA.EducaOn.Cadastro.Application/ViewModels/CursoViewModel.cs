namespace MBA.EducaOn.GestaoConteudo.Application.ViewModels;

public class CursoViewModel
{
    public Guid Id { get;  set; }
    public string Nome { get;  set; }
    public string Descricao { get;  set; }
    public decimal Valor { get;  set; }
    public int CargaHoraria { get;  set; }
    public string PublicoAlvo { get;  set; }
    public string Objetivo { get;  set; }
    public string Requisitos { get;  set; }
    public string ConteudoDescricao { get; set; }
    public int Revisao { get; set; }
    public DateTime DataRevisao { get; set; }
    public DateTime DataCadastro { get;  set; }
    public bool Ativo { get;  set; }

    public IEnumerable<AulaViewModel> Aulas { get; set; }

}
