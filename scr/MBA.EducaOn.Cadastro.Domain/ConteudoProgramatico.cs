using MBA.EducaOn.Core.DomainObjects;

namespace MBA.EducaOn.GestaoConteudo.Domain;

public class ConteudoProgramatico
{
    public ConteudoProgramatico(string conteudoDescricao, int revisao, DateTime dataRevisao)
    {
        ConteudoDescricao = conteudoDescricao;
        Revisao = revisao;
        DataRevisao = dataRevisao;

        Validar();
    }

    public string ConteudoDescricao { get; private set; }
    public int Revisao { get; private set; }
    public DateTime DataRevisao { get; private set; }

    public void Validar()
    {
        Validacoes.ValidarSeVazio(ConteudoDescricao, "A descrição do conteúdo programático não pode estar vazia");
        Validacoes.ValidarTamanho(ConteudoDescricao, DescricaoMaxLength, $"A descrição do conteúdo programático não pode ter mais que {DescricaoMaxLength} caracteres");
        Validacoes.ValidarDataSeNula(DataRevisao, "A data de revisão do conteúdo programático não pode ser nula");
        Validacoes.ValidarSeMenorQue(Revisao, 1, "A revisão do conteúdo programático deve ser maior que zero");
        Validacoes.ValidarDataFutura(DataRevisao, "A data de revisão do conteúdo programático não pode ser futura");
    }

    #region Constants
    public const int DescricaoMaxLength = 1000;
    #endregion
}
