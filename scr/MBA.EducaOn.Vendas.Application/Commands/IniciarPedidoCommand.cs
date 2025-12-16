using FluentValidation;
using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Vendas.Application.Queries.ViewModels;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class IniciarPedidoCommand : Command
{
    public Guid PedidoId { get; private set; }
    public Guid AlunoId { get; private set; }
    public decimal Total { get; private set; }
    public string NomeCartao { get; private set; }
    public string NumeroCartao { get; private set; }
    public string ExpiracaoCartao { get; private set; }
    public string CvvCartao { get; private set; }

    public IniciarPedidoCommand(CarrinhoViewModel carrinhoViewModel)
    {
        PedidoId = carrinhoViewModel.PedidoId;
        AlunoId = carrinhoViewModel.AlunoId;
        Total = carrinhoViewModel.ValorTotal;
        NomeCartao = carrinhoViewModel.Pagamento.NomeCartao;
        NumeroCartao = carrinhoViewModel.Pagamento.NumeroCartao;
        ExpiracaoCartao = carrinhoViewModel.Pagamento.ExpiracaoCartao;
        CvvCartao = carrinhoViewModel.Pagamento.CvvCartao;
    }

    public override bool EhValido()
    {
        ValidationResult = new IniciarPedidoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class IniciarPedidoValidation : AbstractValidator<IniciarPedidoCommand>
{
    public IniciarPedidoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.PedidoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do pedido inválido");

        RuleFor(c => c.NomeCartao)
            .NotEmpty()
            .WithMessage("O nome no cartão não foi informado");

        RuleFor(c => c.NumeroCartao)
            .CreditCard()
            .WithMessage("Número de cartão de crédito inválido");

        RuleFor(c => c.ExpiracaoCartao)
            .NotEmpty()
            .WithMessage("Data de expiração não informada");

        RuleFor(c => c.CvvCartao)
            .Length(3, 4)
            .WithMessage("O CVV não foi preenchido corretamente");
    }
}
