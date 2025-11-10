using MBA.EducaOn.Core.Enumerators;
using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = nameof(TipoUsuario.Administrador))]
public class CursoController : ControllerBase
{
    private readonly ICursoService _cursoService;

    public CursoController(ICursoService cursoService)
    {
        _cursoService = cursoService;
    }

    /// <summary>
    /// Obtém um curso pelo seu identificador único.
    /// </summary>
    /// <param name="id">O identificador único do curso.</param>
    /// <returns>Os detalhes do curso se encontrado; caso contrário, NotFound.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var curso = await _cursoService.ObterPorId(id);

        if (curso == null)
            return NotFound();

        return Ok(curso);
    }

    /// <summary>
    /// Obtém todos os cursos disponíveis.
    /// </summary>
    /// <returns>Uma lista de cursos se encontrados; caso contrário, NotFound.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterTodos()
    {
        var cursos = await _cursoService.ObterTodos();

        if (cursos == null || !cursos.Any())
        {
            return NotFound();
        }

        return Ok(cursos);
    }

    /// <summary>
    /// Adiciona um novo Curso
    /// </summary>
    /// <param name="viewModel">Informe o dados do Curso a ser adicionado</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CursoViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Adicionar([FromBody] CursoViewModel viewModel)
    {
        if (viewModel is null)
            return BadRequest("Parametros invalidos");

        var existingCurso = await _cursoService.ExistAsync(viewModel.Nome);

        if (existingCurso)
        {
            return Problem("Curso Já Cadastrado");
        }

        var result = await _cursoService.Adicionar(viewModel);
        if (result != null)
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);

        return BadRequest("Falha ao tentar cadastrar Curso");
    }

    /// <summary>
    /// Altera dados de um Curso
    /// </summary>
    /// <param name="id">Informe o Id do Curso que deseja alterar</param>
    /// <param name="viewModel">Informe o dados do Curso</param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(CursoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CursoViewModel viewModel)
    {
        if(viewModel is null)
            return BadRequest("Parametros invalidos");

        if (id != viewModel.Id)
        {
            return Problem("ID não corresponde aos dados informados.");
        }

        var existingCurso = await _cursoService.ExistAsync(id);

        if (!existingCurso)
        {
            return NotFound();
        }

        var updatedCurso = await _cursoService.Atualizar(viewModel);

        return Ok(updatedCurso);
    }

    /// <summary>
    /// Deleta um Curso existente
    /// </summary>
    /// <param name="id">Informe o Id do Curso a ser Deletado</param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Problem("ID não corresponde aos dados informados.");
        }

        var existingCurso = await _cursoService.ExistAsync(id);

        if (!existingCurso)
        {
            return NotFound();
        }

        await _cursoService.Deletar(id);

        return Ok($"Curso ID: {id} deleted successfully!");
    }

    /// <summary>
    /// Adiciona Aula a um Curso Cadastrado
    /// </summary>
    /// <param name="id">Informe o Id do Curso que deseja adicionar a Aula</param>
    /// <param name="viewModel">Informe o dados da Aula a ser adicionada</param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(CursoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("aula-adicionar/{id}")]   
    public async Task<IActionResult> AdicionarAula(Guid id, AulaViewModel viewModel)
    {
        if (viewModel is null)
            return BadRequest("Parametros invalidos");

        if (id != viewModel.CursoId)
        {
            return Problem("ID não corresponde aos dados do curso informado.");
        }

        var result = await _cursoService.AdicionarAula(viewModel);

        if (result == null)
            return Problem("Falha ao Adicionar aula");

        return Ok(result);
    }

    /// <summary>
    /// Deleta um Curso existente
    /// </summary>
    /// <param name="id">Informe o Id da Aula a ser Deletada</param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("deletar-aula/{id}")]
    public async Task<IActionResult> DeletarAula(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Problem("ID não corresponde aos dados informados.");
        }

        var existingCurso = await _cursoService.ExistAsync(id);

        if (!existingCurso)
        {
            return NotFound();
        }

        await _cursoService.DeletarAulaAsync(id);

        return Ok($"Aula ID: {id} deleted successfully!");
    }
}