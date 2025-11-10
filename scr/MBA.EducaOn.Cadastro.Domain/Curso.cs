using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Curso : Entity, IAggregateRoot
{
    protected Curso(){}

    public Curso(string nome, string descricao, decimal valor, int cargaHoraria,
        string publicoAlvo, string objetivo, string requisitos, ConteudoProgramatico conteudoProgramatico)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirValor(valor);
        DefinirCargaHoraria(cargaHoraria);
        DefinirPublicoAlvo(publicoAlvo);
        DefinirObjetivo(objetivo);
        DefinirRequisitos(requisitos);
        AlterarConteudoProgramatico(conteudoProgramatico);

        Ativo = true;        
    }

    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Valor { get; private set; }
    public int CargaHoraria { get; private set; }
    public string PublicoAlvo { get; private set; }
    public string Objetivo { get; private set; }
    public string Requisitos { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public ConteudoProgramatico ConteudoProgramatico { get; private set; }

    private List<Aula> _aulas;
    public IReadOnlyCollection<Aula> Aulas => _aulas;

    public void AlteraStado(bool ativo) => Ativo = ativo;

    public void AlterarConteudoProgramatico(ConteudoProgramatico conteudoProgramatico)
    {
        ConteudoProgramatico = conteudoProgramatico;
    }

    public void AdicionarAula(Aula aula)
    {
        if (_aulas == null)
            _aulas = new List<Aula>();

        _aulas.Add(aula);
    }

    public void RemoverAula(Aula aula)
    {
        if (_aulas != null && _aulas.Count > 0)
            _aulas.Remove(aula);
    }

    private void DefinirNome(string nome)
    {
        Validacoes.ValidarSeVazio(nome, "O nome do curso é obrigatório.");
        Validacoes.ValidarTamanho(nome, NomeMaxLength, $"O nome do curso não pode exceder {NomeMaxLength} caracteres.");
        Nome = nome;
    }

    private void DefinirDescricao(string descricao)
    {
        Validacoes.ValidarSeVazio(descricao, "A descrição do curso é obrigatória.");
        Validacoes.ValidarTamanho(descricao, DescricaoMaxLength, $"A descrição do curso não pode exceder {DescricaoMaxLength} caracteres.");
        Descricao = descricao;
    }

    private void DefinirValor(decimal valor)
    {
        Validacoes.ValidarMinimoMaximo(valor, 0, decimal.MaxValue, "O valor do curso não pode ser negativo.");
        Valor = valor;
    }

    private void DefinirCargaHoraria(int cargaHoraria)
    {
        Validacoes.ValidarSeMenorQue(cargaHoraria, 1, "A carga horária do curso deve ser maior que zero.");
        CargaHoraria = cargaHoraria;
    }

    private void DefinirPublicoAlvo(string publicoAlvo)
    {
        Validacoes.ValidarSeVazio(publicoAlvo, "O público alvo do curso é obrigatório.");
        Validacoes.ValidarTamanho(publicoAlvo, PublicoAlvoMaxLength, $"O público alvo do curso não pode exceder {PublicoAlvoMaxLength} caracteres.");
        PublicoAlvo = publicoAlvo;
    }

    private void DefinirObjetivo(string objetivo)
    {
        Validacoes.ValidarSeVazio(objetivo, "O objetivo do curso é obrigatório.");
        Validacoes.ValidarTamanho(objetivo, ObjetivoMaxLength, $"O objetivo do curso não pode exceder {ObjetivoMaxLength} caracteres.");
        Objetivo = objetivo;
    }

    private void DefinirRequisitos(string requisitos)
    {
        Validacoes.ValidarSeVazio(requisitos, "Os requisitos do curso são obrigatórios.");
        Validacoes.ValidarTamanho(requisitos, RequisitosMaxLength, $"Os requisitos do curso não podem exceder {RequisitosMaxLength} caracteres.");
        Requisitos = requisitos;
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants
    public const int NomeMaxLength = 200;
    public const int DescricaoMaxLength = 1000;
    public const int PublicoAlvoMaxLength = 300;
    public const int ObjetivoMaxLength = 500;
    public const int RequisitosMaxLength = 500;
    #endregion
}
