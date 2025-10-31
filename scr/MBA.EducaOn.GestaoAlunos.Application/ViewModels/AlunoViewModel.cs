namespace MBA.EducaOn.GestaoAlunos.Application.ViewModels;

public class AlunoViewModel
{
    public Guid Id { get;  set; }
    public string Nome { get;  set; }
    public string Email { get;  set; }
    public DateTime DataCadastro { get;  set; }
    public bool Ativo { get;  set; }
}
