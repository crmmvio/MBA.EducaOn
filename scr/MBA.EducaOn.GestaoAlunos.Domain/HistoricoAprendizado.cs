namespace MBA.EducaOn.GestaoAlunos.Domain;

public class HistoricoAprendizado
{
    public HistoricoAprendizado(Guid cursoId, Guid alunoId, DateTime dataAprendizado)
    {
        CursoId = cursoId;
        AulaId = alunoId;
        DataAprendizado = dataAprendizado;
    }

    public  Guid CursoId { get; private set; }
    public  Guid AulaId { get; private set; }
    public DateTime DataAprendizado { get; private set; }

}
