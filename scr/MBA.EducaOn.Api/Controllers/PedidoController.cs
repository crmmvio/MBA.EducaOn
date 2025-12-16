using MBA.EducaOn.Api.Controllers.Base;
using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MBA.EducaOn.Vendas.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers
{
    /// <summary>
    /// Controller responsável por expor endpoints relacionados a pedidos do aluno.
    /// Fornece operações para consultar os pedidos de um aluno e outras ações
    /// relacionadas ao agregado de vendas.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Aluno,Administrador")]
    public class PedidoController : ControllerNotificationBase
    {
        private readonly ILogger<CarrinhoController> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IPedidoQueries _pedidoQueries;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="PedidoController"/>.
        /// </summary>
        /// <param name="notifications">Handler de notificações injetado pelo MediatR (esperado ser <see cref="DomainNotificationHandler"/>).</param>
        /// <param name="logger">Logger para registro de informações do controller.</param>
        /// <param name="mediatorHandler">Mediador para envio de comandos e publicação de eventos.</param>
        /// <param name="pedidoQueries">Serviço de queries para recuperar informações de pedidos.</param>
        public PedidoController(INotificationHandler<DomainNotification> notifications,
            ILogger<CarrinhoController> logger,
            IMediatorHandler mediatorHandler,
            IPedidoQueries pedidoQueries) : base(notifications, mediatorHandler)
        {
            _logger = logger;
            _mediatorHandler = mediatorHandler;
            _pedidoQueries = pedidoQueries;
        }

        /// <summary>
        /// Recupera os pedidos do aluno identificado por <paramref name="alunoId"/>.
        /// </summary>
        /// <param name="alunoId">Identificador único do aluno cujos pedidos serão recuperados.</param>
        /// <returns>
        /// Retorna <see cref="NotFoundResult"/> quando não há resultados; caso contrário,
        /// retorna <see cref="OkObjectResult"/> com a coleção de pedidos.
        /// </returns>
        [HttpGet]
        [Route("meus-pedidos")]
        public async Task<IActionResult> ObterPedidos(Guid alunoId)
        {
            var result = await _pedidoQueries.ObterPedidosAluno(alunoId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
