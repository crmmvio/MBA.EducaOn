using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;
using MediatR;

namespace MBA.EducaOn.Vendas.Application.Commands;

public class PedidoCommandHandler :
    IRequestHandler<AdicionarItemPedidoCommand, bool>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMediator _mediator;

    public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
    {
        _pedidoRepository = pedidoRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return false;

        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(message.AlunoId);
        var pedidoItem = new PedidoItem(message.CursoId, message.NomeCurso, message.ValorUnitario);


        if (pedido == null)
        {
            pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.AlunoId);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepository.Adicionar(pedido);
        }
        else
        {
            var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);

            if(pedidoItemExistente)
            {
                //Lanca uma notificacao de erro
                return false;
            }
            else
            {
                _pedidoRepository.AdicionarItem(pedidoItem);
            }
        }

        return await _pedidoRepository.UnitOfWork.Commit();
    }

    private bool ValidarCommand(Command message)
    {
        if (message.EhValido()) return true;

        foreach (var error in message.ValidationResult.Errors)
        {
            // _mediator.Publish(new DomainNotification(message.MessageType, error.ErrorMessage));
        }

        return false;
    }
}
