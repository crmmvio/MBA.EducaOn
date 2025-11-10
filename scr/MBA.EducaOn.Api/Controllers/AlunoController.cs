using MBA.EducaOn.Core.Enumerators;
using MBA.EducaOn.GestaoAlunos.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.EducaOn.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Administrador,Aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunoController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    /// <summary>
    /// Obtém um aluno pelo seu identificador único.
    /// </summary>
    /// <param name="id">O identificador único do aluno.</param>
    /// <returns>Os detalhes do aluno se encontrado; caso contrário, NotFound.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id}")]    
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var aluno = await _alunoService.ObterPorId(id);

        if (aluno == null)
            return NotFound();

        return Ok(aluno);
    }

    /// <summary>
    /// Obtém todos os alunos disponíveis.
    /// </summary>
    /// <returns>Uma lista de alunos se encontrados; caso contrário, NotFound.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterTodos()
    {
        var alunos = await _alunoService.ObterTodos();

        if (alunos == null || !alunos.Any())
        {
            return NotFound();
        }

        return Ok(alunos);
    }

    [HttpPost]
    [Authorize(Roles = nameof(TipoUsuario.Aluno))]
    public IActionResult MatricularCurso()
    {
        return Ok("Teste OK");
    }
}
;