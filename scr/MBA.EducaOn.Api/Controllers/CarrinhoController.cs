using MBA.EducaOn.Core.Communication.Mediator;
using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.Vendas.Application.Commands;
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
public class CarrinhoController : ControllerBase
{
    private readonly ILogger<CarrinhoController> _logger;
    private readonly IMediatorHandler _mediatorHandler;
    private readonly ICursoService _cursoService;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CarrinhoController"/>.
    /// </summary>
    /// <param name="logger">Logger para registro de informações do controller.</param>
    /// <param name="mediatorHandler">Mediador para envio de comandos e publicação de eventos.</param>
    /// <param name="cursoService">Serviço para recuperação de dados de cursos.</param>
    public CarrinhoController(ILogger<CarrinhoController> logger,
        IMediatorHandler mediatorHandler, ICursoService cursoService)
    {
        _logger = logger;
        _mediatorHandler = mediatorHandler;
        _cursoService = cursoService;
    }

    /// <summary>
    /// Adiciona um item ao carrinho para o aluno e o curso especificados.
    /// </summary>
    /// <param name="alunoId">Identificador único do aluno.</param>
    /// <param name="cursoId">Identificador único do curso.</param>
    /// <returns>Um <see cref="IActionResult"/> indicando o resultado da operação.</returns>
    [HttpPost()]
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
}
