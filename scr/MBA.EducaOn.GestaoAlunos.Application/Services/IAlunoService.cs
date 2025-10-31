using MBA.EducaOn.GestaoAlunos.Application.ViewModels;

namespace MBA.EducaOn.GestaoAlunos.Application.Services;

public interface IAlunoService : IDisposable
{
    /// <summary>
    /// Obtém uma aula pelo seu identificador único.
    /// </summary>
    /// <param name="id">O identificador único da aula.</param>
    /// <returns>O objeto <see cref="AlunoViewModel"/> correspondente, se encontrado.</returns>
    Task<AlunoViewModel> ObterPorId(Guid id);

    /// <summary>
    /// Obtém todas as aulas disponíveis.
    /// </summary>
    /// <returns>Uma coleção enumerável de <see cref="AlunoViewModel"/>.</returns>
    Task<IEnumerable<AlunoViewModel>> ObterTodos();

    /// <summary>
    /// Adiciona uma nova aula.
    /// </summary>
    /// <param name="cursoViewModel">O objeto <see cref="AlunoViewModel"/> a ser adicionado.</param>
    Task Adicionar(AlunoViewModel cursoViewModel);

    /// <summary>
    /// Atualiza uma aula existente.
    /// </summary>
    /// <param name="cursoViewModel">O objeto <see cref="AlunoViewModel"/> com os dados atualizados.</param>
    /// <returns>O objeto <see cref="AlunoViewModel"/> atualizado.</returns>
    Task<AlunoViewModel> Atualizar(AlunoViewModel cursoViewModel);
}