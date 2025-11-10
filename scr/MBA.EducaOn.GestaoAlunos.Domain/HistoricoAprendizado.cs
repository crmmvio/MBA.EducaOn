using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class HistoricoAprendizado
{
    protected HistoricoAprendizado() { }

    public HistoricoAprendizado(Guid alunoId, Guid cursoId, DateTime dataAprendizado)
    {
        AulaId = alunoId;
        CursoId = cursoId;
        DataAprendizado = dataAprendizado;
        Validar();
    }

    public Guid AulaId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataAprendizado { get; private set; }

    public void Validar()
    {
        Validacoes.ValidarSeNulo(AulaId, "O ID da aula é obrigatório.");
        Validacoes.ValidarSeNulo(CursoId, "O ID do curso é obrigatório.");
        Validacoes.ValidarDataSeNula(DataAprendizado, "A data de aprendizado é obrigatória.");
    }

    public static HistoricoAprendizado Create()
    {
        return new HistoricoAprendizado();
    }
}
