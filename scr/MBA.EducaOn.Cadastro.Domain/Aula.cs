using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Aula : Entity
{
    protected Aula() { }

    public Aula(Guid cursoId, string codigo, string titulo, string descricao, int ordem)
    {
        CursoId = cursoId;
        DefinirCodigo(codigo);
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);
        DefinirOrdem(ordem);
        Ativo = true;
        _materiais = new List<Material>();
    }

    public string Codigo { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public int Ordem { get; private set; }
    public Guid CursoId { get; private set; }
    public Curso Curso { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }

    private readonly List<Material> _materiais;
    public IReadOnlyCollection<Material> Materiais => _materiais.AsReadOnly();

    private void DefinirCodigo(string codigo)
    {
        Validacoes.ValidarSeVazio(codigo, "O código da aula é obrigatório.");
        Validacoes.ValidarTamanho(codigo, CodigoMaxLength, $"O código da aula não pode exceder {CodigoMaxLength} caracteres.");
        Codigo = codigo;
    }

    private void DefinirTitulo(string titulo)
    {
        Validacoes.ValidarSeVazio(titulo, "O título da aula é obrigatório.");
        Validacoes.ValidarTamanho(titulo, TituloMaxLength, $"O título da aula não pode exceder {TituloMaxLength} caracteres.");

        Titulo = titulo;
    }

    private void DefinirDescricao(string descricao)
    {
        Validacoes.ValidarSeVazio(descricao, "A descrição da aula é obrigatória.");
        Validacoes.ValidarTamanho(descricao, DescricaoMaxLength, $"A descrição da aula não pode exceder {DescricaoMaxLength} caracteres.");

        Descricao = descricao;
    }

    private void DefinirOrdem(int ordem)
    {
        Validacoes.ValidarMinimoMaximo(ordem, 1, int.MaxValue, "A ordem da aula deve ser maior que zero.");
        Ordem = ordem;
    }

    private void DefinirCurso(Curso curso)
    {
        Validacoes.ValidarSeNulo(curso, "O curso associado à aula é obrigatório.");
        CursoId = curso.Id;
        Curso = curso;
    }

    public void AlteraStado(bool ativo) => Ativo = ativo;
    
    public void AdicionarMaterial(Material material)
    {
        _materiais.Add(material);
    }

    public void RemoverMaterial(Material material)
    {
        _materiais.Remove(material);
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants

    public const int CodigoMaxLength = 20;
    public const int TituloMaxLength = 200;
    public const int DescricaoMaxLength = 500;

    #endregion
}
