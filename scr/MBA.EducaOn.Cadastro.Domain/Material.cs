using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class Material : Entity
{
    protected Material() { }

    public Material(string nome, string descricao, string urlArquivo, string urlLinkReferencia, Aula aula)
    {
        DefinirNome(Nome);
        DefinirDescricao(descricao);
        DefinirUrlArquivo(urlArquivo);
        DefinirUrlLinkSiteReferencia(urlLinkReferencia);
        DefinirAula(aula);
    }

    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public string UrlArquivo { get; private set; }
    public string UrlLinkSiteReferencia { get; private set; }
    public Guid AulaId { get; private set; }
    public Aula Aula { get; private set; }

    private void DefinirNome(string nome)
    {
        Validacoes.ValidarSeVazio(nome, "O nome do material é obrigatório.");
        Validacoes.ValidarTamanho(nome, NomeMaxLength, $"O nome do material não pode exceder {NomeMaxLength} caracteres.");
        Nome = nome;
    }

    private void DefinirDescricao(string descricao)
    {
        Validacoes.ValidarSeVazio(descricao, "A descrição do material é obrigatória.");
        Validacoes.ValidarTamanho(descricao, DescricaoMaxLength, $"A descrição do material não pode exceder {DescricaoMaxLength} caracteres.");
        Descricao = descricao;
    }

    private void DefinirUrlArquivo(string urlArquivo)
    {
        Validacoes.ValidarSeVazio(urlArquivo, "A URL do arquivo do material é obrigatória.");
        Validacoes.ValidarTamanho(urlArquivo, UrlArquivoMaxLength, $"A URL do arquivo do material não pode exceder {UrlArquivoMaxLength} caracteres.");
        UrlArquivo = urlArquivo;
    }

    private void DefinirUrlLinkSiteReferencia(string urlLinkSiteReferencia)
    {
        Validacoes.ValidarTamanho(urlLinkSiteReferencia, UrlLinkSiteReferenciaMaxLength, $"A URL do link de referência do material não pode exceder {UrlLinkSiteReferenciaMaxLength} caracteres.");
        UrlLinkSiteReferencia = urlLinkSiteReferencia;
    }

    private void DefinirAula(Aula aula)
    {
        Validacoes.ValidarSeNulo(aula, "A aula associada ao material é obrigatória.");
        AulaId = aula.Id;
        Aula = aula;
    }

    public void Atualizar(string nome, string descricao, string urlArquivo, string urlLinkReferencia)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirUrlArquivo(urlArquivo);
        DefinirUrlLinkSiteReferencia(urlLinkReferencia);
    }

    public override bool EhValido()
    {
        return true;
    }

    #region Constants
    public const int NomeMaxLength = 200;
    public const int DescricaoMaxLength = 500;
    public const int UrlArquivoMaxLength = 1000;
    public const int UrlLinkSiteReferenciaMaxLength = 1000;
    #endregion
}
