using MBA.EducaOn.GestaoConteudo.Application.ViewModels;

namespace MBA.EducaOn.GestaoConteudo.Application.Services;

public interface ICursoService : IDisposable
{
    /// <summary>
    /// Recupera assíncronamente um curso pelo seu identificador único.
    /// </summary>
    /// <param name="id">O identificador único do curso a ser recuperado. Deve ser um Guid válido.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o objeto <see cref="Curso"/> se encontrado; caso contrário, <see langword="null"/>.</returns>
    Task<CursoViewModel> ObterPorId(Guid id);

    /// <summary>
    /// Recupera assíncronamente todos os cursos disponíveis.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém uma coleção enumerável de objetos <see cref="Curso"/>.</returns>
    Task<IEnumerable<CursoViewModel>> ObterTodos();

    /// <summary>
    /// Verifica se existe o Curso cadastrado
    /// </summary>
    /// <param name="id">O identificador único do curso a ser recuperado. Deve ser um Guid válido.</param>
    /// <returns></returns>
    Task<bool> ExistAsync(Guid id);

    /// <summary>
    /// Verifica se existe o Curso cadastrado
    /// </summary>
    /// <param name="nome">Informe o nome do Curso</param>
    /// <returns></returns>
    Task<bool> ExistAsync(string nome);

    /// <summary>
    /// Adiciona um novo curso à coleção.
    /// </summary>
    /// <param name="curso">O curso a ser adicionado. Não pode ser nulo.</param>
    Task<CursoViewModel> Adicionar(CursoViewModel cursoViewModel);

    /// <summary>
    /// Atualiza o curso especificado com novas informações.
    /// </summary>
    /// <param name="curso">O objeto curso contendo os detalhes atualizados. Não pode ser nulo.</param>
    Task<CursoViewModel> Atualizar(CursoViewModel cursoViewModel);

    /// <summary>
    /// Deleta um registro de Curso
    /// </summary>
    /// <param name="id">O identificador único do curso a ser deletado. Deve ser um Guid válido.</param>
    /// <returns></returns>
    Task Deletar(Guid id);

    /// <summary>
    /// Adiciona Aula ao Curso
    /// </summary>
    /// <param name="aulaViewModel">Informe uma instância do objeto Aula preenchida com dados da Aula</param>
    /// <returns></returns>
    Task<CursoViewModel> AdicionarAula(AulaViewModel aulaViewModel);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">O identificador único da Aula a ser deletado. Deve ser um Guid válido.</param>
    /// <returns></returns>
    Task<CursoViewModel> DeletarAulaAsync(Guid id);
}
