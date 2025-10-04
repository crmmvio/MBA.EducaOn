using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Curso : Entity, IAggregateRoot
{
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Valor { get; private set; }
    public int CargaHoraria { get; private set; }
    public string PublicoAlvo { get; private set; }
    public string Objetivo { get; private set; }
    public string Requisitos { get; private set; }
    public Guid CategoriaId { get; private set; }

    public ICollection<ConteudoProgramatico> ConteudosProgramaticos { get; private set; }
    // EF Core
    protected Curso() { }
    public Curso(string nome, string descricao, decimal valor, int cargaHoraria, string publicoAlvo, string objetivo, string requisitos, Guid categoriaId)
    {
        Nome = nome;
        Descricao = descricao;
        Valor = valor;
        CargaHoraria = cargaHoraria;
        PublicoAlvo = publicoAlvo;
        Objetivo = objetivo;
        Requisitos = requisitos;
        CategoriaId = categoriaId;
        ConteudosProgramaticos = new List<ConteudoProgramatico>();
    }
    public void AdicionarConteudoProgramatico(ConteudoProgramatico conteudo)
    {
        ConteudosProgramaticos.Add(conteudo);
    }
    public void RemoverConteudoProgramatico(ConteudoProgramatico conteudo)
    {
        ConteudosProgramaticos.Remove(conteudo);
    }
}
