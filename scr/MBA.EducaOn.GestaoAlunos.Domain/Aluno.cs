using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain;

public class Aluno : Entity, IAggregateRoot
{
    protected Aluno() { }

    public Aluno(Guid id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
        HistoricoAprendizado = HistoricoAprendizado.Create();
        Ativo = true;

        _matriculas = new List<Matricula>();
        _certificados = new List<Certificado>();
    }

    public string Nome { get; private set; }
    public string Email { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }

    public HistoricoAprendizado HistoricoAprendizado { get; set; }

    private readonly List<Matricula> _matriculas;
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    private readonly List<Certificado> _certificados;
    public IReadOnlyCollection<Certificado> Certificados => _certificados.AsReadOnly();

    public void AlteraStatus(bool ativo) => Ativo = ativo;

    public void AtualizarHistorico(HistoricoAprendizado historico)
    {
        HistoricoAprendizado = historico;
    }

    public void AdicionarMatricula(Guid cursoId)
    {
        _matriculas.Add(new Matricula(Id, cursoId, DateTime.Now));
    }

    public void AdicionarCertificado(Guid cursoId, DateTime dataEmissao, string codigo)
    {
        _certificados.Add(new Certificado(Id, cursoId, dataEmissao, codigo));
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants
    public const int NomeMaxLength = 200;
    public const int EmailMaxLength = 200;
    #endregion
}
