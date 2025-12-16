using FluentValidation;
using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class AplicarVoucherPedidoCommand : Command
{
    public Guid AlunoId { get; private set; }
    public string CodigoVoucher { get; private set; }

    public AplicarVoucherPedidoCommand(Guid alunoId, string codigoVoucher)
    {
        AlunoId = alunoId;
        CodigoVoucher = codigoVoucher;
    }

    public override bool EhValido()
    {
        ValidationResult = new AplicarVoucherPedidoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class AplicarVoucherPedidoValidation : AbstractValidator<AplicarVoucherPedidoCommand>
{
    public AplicarVoucherPedidoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do cliente inválido");

        RuleFor(c => c.CodigoVoucher)
            .NotEmpty()
            .WithMessage("O código do voucher não pode ser vazio");
    }
}
