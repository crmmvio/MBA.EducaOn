using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class Matricula : Entity, IAggregateRoot
{
    public Matricula(){}

    public Matricula(Guid alunoId,Guid cursoId, DateTime dataMatricula)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        DataMatricula = dataMatricula;
        Ativo = true;
    }

    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime DataValidade { get; set; }
    public bool Ativo { get; private set; }

    public Aluno Aluno { get; set; }

    public void AlteraStatus(bool ativo) => Ativo = ativo;

    public void Validar()
    {
        Validacoes.ValidarSeNulo(AlunoId, "O ID do aluno é obrigatório.");
        Validacoes.ValidarSeNulo(CursoId, "O ID do curso é obrigatório.");
        Validacoes.ValidarDataSeNula(DataMatricula, "A data de matrícula é obrigatória.");
    }

    public override bool EhValido()
    {
        return true;
    }
}
