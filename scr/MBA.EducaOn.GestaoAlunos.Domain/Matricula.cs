using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class Matricula : Entity, IAggregateRoot
{
    public Matricula(){}

    public Matricula(Guid alunoId, DateTime dataMatricula)
    {
        AlunoId = alunoId;
        DataMatricula = dataMatricula;
        Ativo = true;
    }

    public Guid AlunoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime DataValidade { get; set; }
    public bool Ativo { get; private set; }

    public Aluno Aluno { get; set; }

    public void AlteraStatus(bool ativo) => Ativo = ativo;
}
