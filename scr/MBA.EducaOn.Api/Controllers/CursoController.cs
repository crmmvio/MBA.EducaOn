using MBA.EducaOn.GestaoConteudo.Application.Services;
using MBA.EducaOn.GestaoConteudo.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers;

[ApiController]
[Route("[controller]")]
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


    [HttpPost]
    public IActionResult Adicionar([FromBody] CursoViewModel viewModel)
    {
        return Ok("Curso added successfully!");
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CursoViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return Problem("ID não corresponde aos dados informados.");
        }

        var existingCurso = _cursoService.ObterPorId(id).Result;

        if (existingCurso == null)
        {
            return NotFound();
        }

        var updatedCurso = await _cursoService.Atualizar(viewModel);

        return Ok(updatedCurso);
    }

    [HttpDelete]
    public IActionResult Deletar(Guid id)
    {
        return Ok($"Curso ID: {id} deleted successfully!");
    }
}