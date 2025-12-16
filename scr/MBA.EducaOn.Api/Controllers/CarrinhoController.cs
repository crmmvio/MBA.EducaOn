using MBA.EducaOn.Api.Controllers.Base;
using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.Core.Messages.CommonMessages.Notifications;
using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.Vendas.Application.Commands;
using MBA.EducaOn.Vendas.Application.Queries;
using MBA.EducaOn.Vendas.Application.Queries.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers;

/// <summary>
/// Controller responsável pelas operações relacionadas ao carrinho de compras,
/// como adicionar itens ao carrinho de um aluno.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Aluno")]
public class CarrinhoController : ControllerNotificationBase
{
    private readonly ILogger<CarrinhoController> _logger;
    private readonly IMediatorHandler _mediatorHandler;
    private readonly ICursoService _cursoService;
    private readonly IPedidoQueries _pedidoQueries;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CarrinhoController"/>.
    /// </summary>
    /// <param name="notifications">Handler de notificações injetado pelo MediatR (esperado ser <see cref="DomainNotificationHandler"/></param>
    /// <param name="logger">Logger para registro de informações do controller.</param>
    /// <param name="mediatorHandler">Mediador para envio de comandos e publicação de eventos.</param>
    /// <param name="cursoService">Serviço para recuperação de dados de cursos.</param>
    /// <param name="pedidoQueries">Consultas relacionadas a pedidos.</param>
    public CarrinhoController(INotificationHandler<DomainNotification> notifications,
                              IMediatorHandler mediatorHandler,
                              ILogger<CarrinhoController> logger,
                              ICursoService cursoService,
                              IPedidoQueries pedidoQueries) : base(notifications, mediatorHandler)
    {
        _logger = logger;
        _mediatorHandler = mediatorHandler;
        _cursoService = cursoService;
        _pedidoQueries = pedidoQueries;
    }

    /// <summary>
    /// Adiciona um item ao carrinho para o aluno e o curso especificados.
    /// </summary>
    /// <param name="alunoId">Identificador único do aluno.</param>
    /// <param name="cursoId">Identificador único do curso.</param>
    /// <returns>Um <see cref="IActionResult"/> indicando o resultado da operação.</returns>
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("adicionar-item/{alunoId}/{cursoId}")]
    public async Task<IActionResult> AdicionarItem(Guid alunoId, Guid cursoId)
    {
        if (alunoId == Guid.Empty)
        {
            return BadRequest("AlunoId invalido");
        }

        var curso = await _cursoService.ObterPorId(cursoId);
        if (curso == null)
        {
            return NotFound("Curso nao encontrado");
        }

        var command = new AdicionarItemPedidoCommand(alunoId, cursoId, curso.Nome, curso.Valor);
        var result = await _mediatorHandler.EnviarComando(command);

        if (!result)
        {
            return BadRequest("Erro ao adicionar item ao carrinho");
        }

        return Ok("Item adicionado com sucesso!");
    }

    /// <summary>
    /// Remove um item do carrinho para o aluno e o curso especificados.
    /// </summary>
    /// <param name="alunoId">Identificador único do aluno cujo carrinho será alterado.</param>
    /// <param name="cursoId">Identificador único do curso a ser removido do carrinho.</param>
    /// <returns>
    /// Redireciona para a ação "Index" quando a operação for válida; caso contrário,
    /// retorna a view "Index" com o carrinho do cliente.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("remover-item")]
    public async Task<IActionResult> RemoverItem(Guid alunoId, Guid cursoId)
    {
        var curso = await _cursoService.ObterPorId(cursoId);
        if (curso == null) return BadRequest();

        var command = new RemoverItemPedidoCommand(alunoId, cursoId);
        await _mediatorHandler.EnviarComando(command);

        if (!OperacaoValida())
        {
            return BadRequest("Erro ao remover item ao carrinho");
        }

        return Ok("Item removido com sucesso!");
    }

    /// <summary>
    /// Aplica um voucher ao carrinho do aluno especificado.
    /// </summary>
    /// <param name="alunoId">Identificador único do aluno que receberá o voucher.</param>
    /// <param name="voucherCodigo">Código do voucher a ser aplicado.</param>
    /// <returns>Um <see cref="IActionResult"/> indicando o resultado da operação.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("aplicar-voucher")]
    public async Task<IActionResult> AplicarVoucher(Guid alunoId, string voucherCodigo)
    {
        var command = new AplicarVoucherPedidoCommand(alunoId, voucherCodigo);
        await _mediatorHandler.EnviarComando(command);

        if (!OperacaoValida())
        {
            return BadRequest("Erro ao Aplicar vouche ao carrinho");
        }

        return Ok("Voucher aplicado com sucesso!");
    }

    /// <summary>
    /// Recupera um resumo do carrinho de compras atual do aluno especificado.
    /// </summary>
    /// <remarks>Este endpoint está disponível via HTTP POST em <c>resumo-da-compra</c>. Use este método para obter o estado mais recente do carrinho de um aluno, incluindo itens e totais.</remarks>
    /// <param name="alunoId">Identificador único do aluno cujo resumo do carrinho é solicitado. Deve ser um <see cref="Guid"/> válido.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo o resumo do carrinho se encontrado; caso contrário, retorna <see cref="NotFoundResult"/>.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("resumo-da-compra")]
    public async Task<IActionResult> ResumoDaCompra(Guid alunoId)
    {
        var result = await _pedidoQueries.ObterCarrinhoAluno(alunoId);

        if (result == null)
        {
            return NotFound("Não encontrado pedido para o Aluno informado.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Inicia um novo pedido com base no carrinho de compras e nas informações de pagamento fornecidas.
    /// </summary>
    /// <remarks>
    /// Esta ação espera uma requisição POST para a rota "iniciar-pedido". O método valida a operação antes de
    /// retornar um resultado. Se a operação não for válida, retorna um BadRequest com uma mensagem de erro.
    /// </remarks>
    /// <param name="carrinhoViewModel">
    /// ViewModel contendo o carrinho do aluno e os dados de pagamento. Não deve ser <c>null</c>.
    /// A propriedade <see cref="CarrinhoViewModel.AlunoId"/> identifica o aluno para o qual o pedido será iniciado.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> indicando o resultado da operação. Retorna <see cref="OkObjectResult"/> se o pedido
    /// for iniciado com sucesso; caso contrário, retorna um <see cref="BadRequestObjectResult"/> com a mensagem de erro.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("iniciar-pedido")]
    public async Task<IActionResult> IniciarPedido(CarrinhoViewModel carrinhoViewModel)
    {
        var carrinho = await _pedidoQueries.ObterCarrinhoAluno(carrinhoViewModel.AlunoId);

        var command = new IniciarPedidoCommand(carrinho);
        await _mediatorHandler.EnviarComando(command);

        if (!OperacaoValida())
        {
            return BadRequest("Erro ao Aplicar vouche ao carrinho");
        }

        return Ok("Voucher aplicado com sucesso!");
    }
}
