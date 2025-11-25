using FluentValidation;
using MBA.EducaOn.Core.Messages;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class AdicionarItemPedidoCommand : Command
{
    public AdicionarItemPedidoCommand(Guid alunoId, Guid cursoId, string nomeCurso, decimal valorUnitario)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        NomeCurso = nomeCurso;
        ValorUnitario = valorUnitario;
    }

    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public string NomeCurso { get; private set; }
    public decimal ValorUnitario { get; private set; }

    override public bool EhValido()
    {
        ValidationResult = new AdicionarItemPedidoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class AdicionarItemPedidoValidation : AbstractValidator<AdicionarItemPedidoCommand>
{
    public AdicionarItemPedidoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("O código do aluno não pode ser vazio");

        RuleFor(c => c.CursoId)
            .NotEqual(Guid.Empty)
            .WithMessage("O código do curso não pode ser vazio");

        RuleFor(c => c.NomeCurso)
            .NotEmpty()
            .WithMessage("O nome do curso não foi informado");

        RuleFor(c => c.ValorUnitario)
            .GreaterThan(0)
            .WithMessage("O valor unitário deve ser maior que zero");
    }
}