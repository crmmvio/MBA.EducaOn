namespace MBA.EducaOn.Vendas.Domain;

/// <summary>
/// Tipo de desconto aplicado pelo voucher
/// </summary>
public enum TipoDescontoVoucher
{
    /// <summary>
    /// Desconto em porcentagem
    /// </summary>
    Porcentagem = 0,

    /// <summary>
    /// Desconto em valor fixo
    /// </summary>
    Valor = 1
}
