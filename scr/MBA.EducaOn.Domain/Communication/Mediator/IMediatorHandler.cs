using MBA.EducaOn.Core.Messages;
using MBA.EducaOn.Core.Messages.CommonMessages.DomainEvents;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;

namespace MBA.EducaOn.Core.Communication.Mediator;

/// <summary>
/// Interface responsável por atuar como mediador para publicação de eventos, envio de comandos
/// e publicação de notificações dentro do contexto da aplicação.
/// </summary>
public interface IMediatorHandler 
{
    /// <summary>
    /// Publica um evento no barramento de eventos.
    /// </summary>
    /// <typeparam name="T">Tipo do evento que herda de <see cref="Event"/>.</typeparam>
    /// <param name="evento">Instância do evento a ser publicada.</param>
    /// <returns>Uma tarefa assíncrona que representa a operação de publicação.</returns>
    Task PublicarEvento<T>(T evento) where T : Event;

    /// <summary>
    /// Envia um comando para processamento e retorna resultado indicando sucesso ou falha.
    /// </summary>
    /// <typeparam name="T">Tipo do comando que herda de <see cref="Command"/>.</typeparam>
    /// <param name="comando">Instância do comando a ser enviada.</param>
    /// <returns>Uma tarefa assíncrona que retorna <c>true</c> se o comando foi processado com sucesso; caso contrário <c>false</c>.</returns>
    Task<bool> EnviarComando<T>(T comando) where T : Command;

    /// <summary>
    /// Publica uma notificação de domínio (mensagem de erro/aviso) para os handlers registrados.
    /// </summary>
    /// <typeparam name="T">Tipo da notificação que herda de <see cref="DomainNotification"/>.</typeparam>
    /// <param name="notificacao">Instância da notificação a ser publicada.</param>
    /// <returns>Uma tarefa assíncrona que representa a operação de publicação da notificação.</returns>
    Task PublicarNotificacao<T>(T notificacao) where T : DomainNotification;

    /// <summary>
    /// Publica um DomainEvent para os handlers interessados.
    /// </summary>
    /// <typeparam name="T">Tipo do domain event que herda de <see cref="DomainEvent"/>.</typeparam>
    /// <param name="notificacao">Instância do domain event a ser publicada.</param>
    /// <returns>Uma tarefa assíncrona que representa a operação de publicação do domain event.</returns>
    Task PublicarDomainEvent<T>(T notificacao) where T : DomainEvent;
}
