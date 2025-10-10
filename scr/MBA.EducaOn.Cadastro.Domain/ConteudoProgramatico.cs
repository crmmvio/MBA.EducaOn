namespace MBA.EducaOn.GestaoConteudo.Domain;

public class ConteudoProgramatico
{
    public ConteudoProgramatico(string conteudoDescricao, int revisao, DateTime dataRevisao)
    {
        ConteudoDescricao = conteudoDescricao;
        Revisao = revisao;
        DataRevisao = dataRevisao;
    }

    public string ConteudoDescricao { get; private set; }
    public int Revisao { get; private set; }
    public DateTime DataRevisao { get; private set; }

    #region Constants
    public const int DescricaoMaxLength = 1000;
    #endregion
}
