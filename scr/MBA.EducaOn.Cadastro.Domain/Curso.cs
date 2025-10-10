using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Curso : Entity, IAggregateRoot
{    
    protected Curso() { }

    public Curso(string nome, string descricao, decimal valor, int cargaHoraria, string publicoAlvo, string objetivo, string requisitos, ConteudoProgramatico conteudoProgramatico)
    {
        Nome = nome;
        Descricao = descricao;
        Valor = valor;
        CargaHoraria = cargaHoraria;
        PublicoAlvo = publicoAlvo;
        Objetivo = objetivo;
        Requisitos = requisitos;
        ConteudoProgramatico = conteudoProgramatico;
        Ativo = true;
        Aulas = new List<Aula>();
    }

    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Valor { get; private set; }
    public int CargaHoraria { get; private set; }
    public string PublicoAlvo { get; private set; }
    public string Objetivo { get; private set; }
    public string Requisitos { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public ConteudoProgramatico ConteudoProgramatico { get; private set; }
    public bool Ativo { get; private set; }

    public ICollection<Aula> Aulas { get; private set; }

    public void AlteraStado(bool ativo) => Ativo = ativo;

    public void AlterarConteudoProgramatico(ConteudoProgramatico conteudoProgramatico)
    {
        ConteudoProgramatico = conteudoProgramatico;
    }

    public void AdicionarAula(Aula aulta)
    {
        Aulas.Add(aulta);
    }
    public void RemoverAula(Aula aula)
    {
        Aulas.Remove(aula);
    }

    #region Constants
    public const int NomeMaxLength = 200;
    public const int DescricaoMaxLength = 1000;
    public const int PublicoAlvoMaxLength = 300;
    public const int ObjetivoMaxLength = 500;
    public const int RequisitosMaxLength = 500;
    #endregion
}
