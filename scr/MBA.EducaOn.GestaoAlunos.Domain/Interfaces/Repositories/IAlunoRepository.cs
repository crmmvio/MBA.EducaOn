using MBA.EducaOn.Core.Data;

namespace MBA.EducaOn.GestaoAlunos.Domain.Interfaces.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    /// <summary>
    /// Recupera assíncronamente um Aluno pelo seu identificador único.
    /// </summary>
    /// <param name="id">O identificador único do Aluno a ser recuperado. Deve ser um Guid válido.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o objeto <see cref="Aluno"/> se encontrado; caso contrário, <see langword="null"/>.</returns>
    Task<Aluno?> ObterPorIdAsync(Guid id);

    /// <summary>
    /// Recupera assíncronamente todos os Alunos disponíveis.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém uma coleção enumerável de objetos <see cref="Aluno"/>.</returns>
    Task<IEnumerable<Aluno>> ObterTodosAsync();

    /// <summary>
    /// Adiciona um novo Aluno à coleção.
    /// </summary>
    /// <param name="Aluno">O Aluno a ser adicionado. Não pode ser nulo.</param>
    void Adicionar(Aluno Aluno);

    /// <summary>
    /// Atualiza o Aluno especificado com novas informações.
    /// </summary>
    /// <param name="Aluno">O objeto Aluno contendo os detalhes atualizados. Não pode ser nulo.</param>
    void Atualizar(Aluno Aluno);
}
