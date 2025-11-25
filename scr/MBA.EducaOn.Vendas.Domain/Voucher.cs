using FluentValidation.Results;
using MBA.EducaOn.Core.DomainObjects;
using MBA.EducaOn.Vendas.Domain.Validators;

namespace MBA.EducaOn.Vendas.Domain;

public class Voucher : Entity
{
    public string Codigo { get; private set; }
    public decimal? Percentual { get; private set; }
    public decimal? ValorDesconto { get; private set; }
    public int Quantidade { get; private set; }
    public TipoDescontoVoucher TipoDescontoVoucher { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataUtilizacao { get; private set; }
    public DateTime DataValidade { get; private set; }
    public bool Ativo { get; private set; }
    public bool Utilizado { get; private set; }

    public ICollection<Pedido> Pedidos { get; set; }

    internal ValidationResult ValidarSeAplicavel()
    {
        return new VoucherAplicavelValidation().Validate(this);
    }

    #region Constants
    public const int CodigoMaxLength = 100;
    #endregion
}
