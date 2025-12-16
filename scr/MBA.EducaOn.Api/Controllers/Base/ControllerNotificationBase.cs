using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers.Base;

/// <summary>
/// Classe base para controllers que utilizam o mecanismo de notificações de domínio.
/// Fornece utilitários para verificar se há notificações, obter mensagens de erro e
/// notificar erros através do mediador de domínio.
/// </summary>
public abstract class ControllerNotificationBase : ControllerBase
{
    private readonly DomainNotificationHandler _notifications;
    private readonly IMediatorHandler _mediatorHandler;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ControllerNotificationBase"/>.
    /// </summary>
    /// <param name="notifications">Handler de notificações injetado pelo MediatR (esperado ser <see cref="DomainNotificationHandler"/>).</param>
    /// <param name="mediatorHandler">Mediador usado para publicar notificações e eventos.</param>
    protected ControllerNotificationBase(INotificationHandler<DomainNotification> notifications, IMediatorHandler mediatorHandler)
    {
        _notifications = (DomainNotificationHandler)notifications;
        _mediatorHandler = mediatorHandler;
    }

    /// <summary>
    /// Indica se a operação corrente é válida (não possui notificações de erro).
    /// </summary>
    /// <returns><c>true</c> quando não houver notificações; caso contrário <c>false</c>.</returns>
    protected bool OperacaoValida()
    {
        return !_notifications.TemNotificacao();
    }

    /// <summary>
    /// Recupera as mensagens de erro das notificações de domínio registradas.
    /// </summary>
    /// <returns>Uma coleção de mensagens de erro (<see cref="string"/>).</returns>
    protected IEnumerable<string> ObterMensagensErro()
    {
        return _notifications.ObterNotificacoes().Select(c => c.Value).ToList();
    }

    /// <summary>
    /// Publica uma notificação de erro de domínio contendo um código e uma mensagem.
    /// </summary>
    /// <param name="codigo">Código identificador da notificação.</param>
    /// <param name="mensagem">Mensagem descritiva da notificação.</param>
    protected void NotificarErro(string codigo, string mensagem)
    {
        _mediatorHandler.PublicarNotificacao(new DomainNotification(codigo, mensagem));
    }
}
