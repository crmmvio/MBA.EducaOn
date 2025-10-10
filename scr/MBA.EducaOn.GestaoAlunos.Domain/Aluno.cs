using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoAlunos.Domain
{
    public class Aluno : Entity, IAggregateRoot
    {
        public Aluno() { }

        public Aluno(Guid id, string nome, string email, HistoricoAprendizado historicoAprendizado)
        {
            Id = id;
            Nome = nome;
            Email = email;
            HistoricoAprendizado = historicoAprendizado;
            Ativo = true;
        }

        public string Nome { get; private set; }
        public string Email { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public bool Ativo { get; private set; }

        public HistoricoAprendizado HistoricoAprendizado { get; set; }
        public ICollection<Matricula> Matriculas { get; set; }
        public ICollection<Certificado> Certificados { get; set; }

        public void AlteraStatus(bool ativo) => Ativo = ativo;

        public void AtualizarHistorico(HistoricoAprendizado historico)
        {
            HistoricoAprendizado = historico;
        }

        #region Constants
        public const int NomeMaxLength = 200;
        public const int EmailMaxLength = 200;
        #endregion
    }
}
