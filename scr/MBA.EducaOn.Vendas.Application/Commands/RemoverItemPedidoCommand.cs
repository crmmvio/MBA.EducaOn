using FluentValidation;
using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class RemoverItemPedidoCommand : Command
{
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }

    public RemoverItemPedidoCommand(Guid alunoId, Guid cursoId)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
    }

    public override bool EhValido()
    {
        ValidationResult = new RemoverItemPedidoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class RemoverItemPedidoValidation : AbstractValidator<RemoverItemPedidoCommand>
{
    public RemoverItemPedidoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.CursoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do curso inválido");
    }
}
