using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Aula : Entity
{
    public Aula() { }

    public Aula(string codigo, string titulo, string descricao, int ordem, Guid cursoId)
    {
        Codigo = codigo;
        Titulo = titulo;
        Descricao = descricao;
        Ordem = ordem;
        CursoId = cursoId;
        
        Ativo = true;
    }

    public string Codigo { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public int Ordem { get; private set; }
    public Guid CursoId { get; private set; }
    public Curso Curso { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }
    
    public void AlteraStado(bool ativo) => Ativo = ativo;

    #region Constants

    public const int CodigoMaxLength = 20;
    public const int TituloMaxLength = 200;
    public const int DescricaoMaxLength = 500;

    #endregion
}
