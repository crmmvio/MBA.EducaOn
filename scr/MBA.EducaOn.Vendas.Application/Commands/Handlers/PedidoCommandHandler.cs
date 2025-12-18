using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.DomainObjects.Dto;
using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Core.Messages.CommonMessages.IntegrationEvents;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MBA.EducaOn.Vendas.Application.Events;
using MBA.EducaOn.Vendas.Domain;
using MBA.EducaOn.Vendas.Domain.Interfaces;
using MediatR;

namespace MBA.EducaOn.Vendas.Application.Commands.Handlers;

public class PedidoCommandHandler :
    IRequestHandler<AdicionarItemPedidoCommand, bool>,
    IRequestHandler<RemoverItemPedidoCommand, bool>,
    IRequestHandler<AplicarVoucherPedidoCommand, bool>,
    IRequestHandler<IniciarPedidoCommand, bool>,
    IRequestHandler<FinalizarPedidoCommand, bool>,
    IRequestHandler<CancelarProcessamentoPedidoCommand, bool>,
    IRequestHandler<CancelarProcessamentoPedidoNotificarAlunoCommand, bool>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMediatorHandler _mediatorHandler;

    public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediatorHandler mediatorHandler)
    {
        _pedidoRepository = pedidoRepository;
        _mediatorHandler = mediatorHandler;
    }

    public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return false;

        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(message.AlunoId);
        var pedidoItem = new PedidoItem(message.CursoId, message.NomeCurso, message.ValorUnitario);

        if (pedido == null)
        {
            var codigoPedido = await _pedidoRepository.ObterProximoCodigo();
            pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.AlunoId, codigoPedido);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepository.Adicionar(pedido);
        }
        else
        {
            var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);

            if (pedidoItemExistente)
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

    public async Task<bool> Handle(RemoverItemPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!ValidarCommand(message)) return false;

        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(message.AlunoId);

        if (pedido == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
            return false;
        }

        var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, message.CursoId);

        if (pedidoItem != null && !pedido.PedidoItemExistente(pedidoItem))
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Item do pedido não encontrado!"));
            return false;
        }

        pedido.RemoverItem(pedidoItem);

        _pedidoRepository.RemoverItem(pedidoItem);
        _pedidoRepository.Atualizar(pedido);

        return await _pedidoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> Handle(AplicarVoucherPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!ValidarCommand(message)) return false;

        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(message.AlunoId);

        if (pedido == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
            return false;
        }

        var voucher = await _pedidoRepository.ObterVoucherPorCodigo(message.CodigoVoucher);

        if (voucher == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Voucher não encontrado!"));
            return false;
        }

        var voucherAplicacaoValidation = pedido.AplicarVoucher(voucher);
        if (!voucherAplicacaoValidation.IsValid)
        {
            foreach (var error in voucherAplicacaoValidation.Errors)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification(error.ErrorCode, error.ErrorMessage));
            }

            return false;
        }

        pedido.AdicionarEvento(new VoucherAplicadoPedidoEvent(message.AlunoId, pedido.Id, voucher.Id));

        _pedidoRepository.Atualizar(pedido);

        return await _pedidoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> Handle(IniciarPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!ValidarCommand(message)) return false;

        var pedido = await _pedidoRepository.ObterPedidoRascunhoPorAlunoId(message.AlunoId);
        pedido.IniciarPedido();

        var itensList = new List<Item>();
        pedido.PedidoItems.ToList().ForEach(i => itensList.Add(new Item { Id = i.CursoId, CursonNome = i.CursoNome}));
        var listaProdutosPedido = new ListaCursosPedido{ PedidoId = pedido.Id, Itens = itensList };

        pedido.AdicionarEvento(new PedidoIniciadoEvent(pedido.Id, pedido.AlunoId, listaProdutosPedido, pedido.ValorTotal, 
            message.NomeCartao, message.NumeroCartao, message.ExpiracaoCartao, message.CvvCartao));

        _pedidoRepository.Atualizar(pedido);
        return await _pedidoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> Handle(FinalizarPedidoCommand message, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);

        if (pedido == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
            return false;
        }

        pedido.FinalizarPedido();

        pedido.AdicionarEvento(new PedidoFinalizadoEvent(message.PedidoId));
        return await _pedidoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> Handle(CancelarProcessamentoPedidoNotificarAlunoCommand message, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);

        if (pedido == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
            return false;
        }

        var itensList = new List<Item>();
        pedido.PedidoItems.ToList().ForEach(i => itensList.Add(new Item { Id = i.CursoId, CursonNome = i.CursoNome }));
        var listaCursosPedido = new ListaCursosPedido { PedidoId = pedido.Id, Itens = itensList };

        pedido.AdicionarEvento(new PedidoProcessamentoCanceladoEvent(pedido.Id, pedido.AlunoId, listaCursosPedido));
        pedido.TornarRascunho();

        return await _pedidoRepository.UnitOfWork.Commit();
    }

    public async Task<bool> Handle(CancelarProcessamentoPedidoCommand message, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);

        if (pedido == null)
        {
            await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
            return false;
        }

        pedido.TornarRascunho();

        return await _pedidoRepository.UnitOfWork.Commit();
    }

    private bool ValidarCommand(Command message)
    {
        if (message.EhValido()) return true;

        foreach (var error in message.ValidationResult.Errors)
        {
            _mediatorHandler.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));
        }

        return false;
    }
}
