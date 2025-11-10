using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class Certificado : Entity
{
    protected Certificado(){}

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

    public void Validar()
    {
        Validacoes.ValidarSeVazio(Codigo, "O código do certificado é obrigatório.");
        Validacoes.ValidarTamanho(Codigo, CodigoMaxLength, $"O código do certificado não pode exceder {CodigoMaxLength} caracteres.");
        Validacoes.ValidarSeNulo(AlunoId, "O ID do aluno é obrigatório.");
        Validacoes.ValidarSeNulo(CursoId, "O ID do curso é obrigatório");
        Validacoes.ValidarDataSeNula(DataEmissao, "A data de emissão do certificado é obrigatória.");
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants
    public const int CodigoMaxLength = 20;
    #endregion

}
