using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class Certificado : Entity
{
    public Certificado(){}

    public Certificado(Guid alunoId, Guid cursoId, DateTime dataEmissao, string codigo)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        DataEmissao = dataEmissao;
        Codigo = codigo;
    }

    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataEmissao { get; private set; }
    public string Codigo { get; private set; }

    public Aluno Aluno { get; set; }

    #region Constants
    public const int CodigoMaxLength = 20;
    #endregion

}
